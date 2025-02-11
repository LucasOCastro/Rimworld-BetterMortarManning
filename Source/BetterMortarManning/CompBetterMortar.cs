using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

// ReSharper disable InconsistentNaming

namespace BetterMortarManning
{
    public class CompBetterMortar : ThingComp
    {
        private static readonly CachedTexture ToggleTurretIcon = new CachedTexture("UI/Gizmos/ToggleTurret");

        private CompMannable _mannable;
        private CompMannable Mannable
        {
            get
            {
                if (_mannable != null)
                    return _mannable;
                
                if (!parent.TryGetComp(out _mannable))
                    throw new System.Exception($"{typeof(CompBetterMortar).FullName} requires {nameof(CompMannable)}. This comp should be added automatically only, and not manually.");
                
                return _mannable;
            }
        }
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (parent.Faction != Faction.OfPlayer)
                yield break;
            
            yield return new Command_Action
            {
                icon = ToggleTurretIcon,
                defaultLabel = "CrazyMalk_CommandManAll",
                defaultDesc = "CrazyMalk_CommandManAllDesc",
                action = ManAll
            };
        }

        private void ManAll()
        {
            IReadOnlyList<Pawn> allPawnsSpawned = parent.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
            allPawnsSpawned.
        }

        private bool CanMan(Pawn pawn)
        {
            if (!pawn.RaceProps.ToolUser)
                return false;
            
            if (!pawn.CanReserveAndReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
                return false;
                
            if (Mannable.Props.manWorkType != WorkTags.None && pawn.WorkTagIsDisabled(Mannable.Props.manWorkType))
                return false;
                
            return true;
        }
    }
}