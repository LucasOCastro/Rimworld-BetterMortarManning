using System.Collections.Generic;
using RimWorld;
using Verse;

// ReSharper disable InconsistentNaming

namespace BetterMortarManning
{
    public class CompBetterMortar : ThingComp
    {
        private static readonly CachedTexture ToggleTurretIcon = new("UI/Gizmos/ToggleTurret");
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (parent.Faction != Faction.OfPlayer)
                yield break;

            yield return new Command_Action
            {
                icon = ToggleTurretIcon.Texture,
                defaultLabel = "CrazyMalk_CommandManAll".Translate(),
                defaultDesc = "CrazyMalk_CommandManAllDesc".Translate(),
                action = MortarManningPlanner.ManSelected
            };
        }
    }
}