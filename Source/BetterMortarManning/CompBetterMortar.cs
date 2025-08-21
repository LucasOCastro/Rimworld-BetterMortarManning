using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BetterMortarManning
{
    public class CompBetterMortar : ThingComp
    {
        private static readonly CachedTexture ManAllIcon = new("UI/Commands/SquadAttack");
        private static readonly CachedTexture AreaAttackIcon = new("UI/Commands/Attack");
        
        private CompMannable? _mannable;
        public CompMannable Mannable => _mannable ??= parent.GetComp<CompMannable>();
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (parent.Faction != Faction.OfPlayer)
                yield break;

            if (!Mannable.MannedNow)
                yield return GenManAllCommand();
            else
                yield return GenAttackAreaCommand();
        }

        private Gizmo GenAttackAreaCommand()
        {
            var turret = (Building_Turret)parent;
            var command = new Command_VerbAreaTarget
            {
                icon = AreaAttackIcon.Texture,
                defaultLabel = "CrazyMalk_CommandAttackArea".Translate(),
                defaultDesc = "CrazyMalk_CommandAttackAreaDesc".Translate(),
                verb = turret.AttackVerb,
                drawRadius = false,
                requiresAvailableVerb = false
            };
            if (!MortarUtility.CanShoot(turret))
            {
                command.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
            }

            return command;
        }

        private static Gizmo GenManAllCommand()
        {
            return new Command_Action
            {
                icon = ManAllIcon.Texture,
                defaultLabel = "CrazyMalk_CommandManAll".Translate(),
                defaultDesc = "CrazyMalk_CommandManAllDesc".Translate(),
                action = MortarManningPlanner.ManSelected
            };
        }
    }
}