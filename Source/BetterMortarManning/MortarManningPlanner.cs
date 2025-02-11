using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BetterMortarManning
{
    public static class MortarManningPlanner
    {
        private static int _lastTick = -1;
        public static void ManSelected()
        {
            // ManSelected might be called by multiple things at the same time
            // so we need to make sure we don't do the same planning more than once
            int currentTick = Find.TickManager.TicksGame;
            if (_lastTick == currentTick)
                return;
            _lastTick = currentTick;
            
            var busyPawns = SimplePool<HashSet<Pawn>>.Get()!;
            var successfulMortars = SimplePool<List<CompMannable>>.Get()!;
            var failedMortars = SimplePool<List<CompMannable>>.Get()!;
            var selectedMortars = SimplePool<List<CompMannable>>.Get()!;
            selectedMortars.AddRange(GetSelectedMortars());
            
            foreach (var mortar in selectedMortars)
            {
                var pawn = GetClosestFreePawnToMortar(mortar, busyPawns);
                if (pawn == null)
                {
                    // play the little message on the top, not a letter
                    if (mortar.parent.Faction == Faction.OfPlayer)
                        failedMortars.Add(mortar);
                    continue;
                }
                
                if (mortar.parent.Faction == Faction.OfPlayer)
                    successfulMortars.Add(mortar);
                
                busyPawns.Add(pawn);
                MortarUtility.CommandMan(mortar, pawn);
            }

            NotifySuccess(successfulMortars);
            NotifyFail(failedMortars);
            PlaySound(successfulMortars.Count > 0);
            
            SimplePool<HashSet<Pawn>>.Return(busyPawns);
            SimplePool<List<CompMannable>>.Return(successfulMortars);
            SimplePool<List<CompMannable>>.Return(failedMortars);
            SimplePool<List<CompMannable>>.Return(selectedMortars); 
        }
        
        private static IEnumerable<CompMannable> GetSelectedMortars()
        {
            foreach (var thing in Find.Selector.SelectedObjects.OfType<ThingWithComps>())
                if (thing.TryGetComp(out CompMannable mannable))
                    yield return mannable;
        }

        private static Pawn? GetClosestFreePawnToMortar(CompMannable mortar, HashSet<Pawn> busy)
        {
            var pawns = mortar.parent.Map.mapPawns.SpawnedPawnsInFaction(mortar.parent.Faction);
            return pawns
                .Where(p => !busy.Contains(p) && MortarUtility.CanMan(mortar, p))
                .OrderBy(p => p.Position.DistanceToSquared(mortar.parent.Position))
                .FirstOrDefault();
        }

        private static void PlaySound(bool success)
        {
            var sound = success ? SoundDefOf.Tick_High : SoundDefOf.Crunch;
            sound.PlayOneShotOnCamera();
        }

        private static void NotifySuccess(List<CompMannable> selectedMortars)
        {
            if (selectedMortars.Count == 0)
                return;
            
            // TODO Select or point to the chosen pawns
        }
        
        private static void NotifyFail(List<CompMannable> failedMortars)
        {
            if (failedMortars.Count == 0)
                return;

            var targets = new LookTargets(failedMortars.Select(m => m.parent));
            string message = failedMortars.Count == 1 ?
                "CrazyMalk_NoFreePawnsToMan" :
                "CrazyMalk_NoFreePawnsToManPlural";
            Messages.Message(message.Translate(), targets, MessageTypeDefOf.RejectInput, false);
        }
    }
}