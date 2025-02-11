using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

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
            var failedMortars = SimplePool<List<CompMannable>>.Get()!;
            var selectedMortars = SimplePool<List<CompMannable>>.Get()!;
            selectedMortars.AddRange(GetSelectedMortars());
            
            foreach (var mortar in selectedMortars)
            {
                var pawn = GetClosestFreePawnToMortar(mortar, busyPawns);
                if (pawn == null)
                {
                    // play the little message on the top, not a letter
                    failedMortars.Add(mortar);
                    continue;
                }
                
                busyPawns.Add(pawn);
                MortarUtility.CommandMan(mortar, pawn);
            }

            PlayFailMessage(failedMortars);
            
            SimplePool<HashSet<Pawn>>.Return(busyPawns);
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

        private static void PlayFailMessage(List<CompMannable> failedMortars)
        {
            if (failedMortars.Count == 0)
                return;

            var targets = new LookTargets(failedMortars.Select(m => m.parent));
            string message = failedMortars.Count == 1 ?
                "CrazyMalk_NoFreePawnsToMan" :
                "CrazyMalk_NoFreePawnsToManPlural";

            Messages.Message(message, targets, MessageTypeDefOf.RejectInput, false);
        }
    }
}