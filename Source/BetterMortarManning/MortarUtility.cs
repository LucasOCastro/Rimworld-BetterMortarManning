using RimWorld;
using Verse;
using Verse.AI;

namespace BetterMortarManning
{
    public static class MortarUtility
    {
        public static bool IsMortar(Building_Turret turret)
        {
            return turret.def.building.IsMortar;
        }
        
        public static bool IsMortarOrProjectileFliesOverhead(Building_Turret turret)
        {
            return turret.AttackVerb.ProjectileFliesOverhead() || IsMortar(turret);
        }
        
        public static bool CanShoot(Building_Turret turret)
        {
            if (!turret.Spawned)
                return false;
            
            if (IsMortarOrProjectileFliesOverhead(turret) && turret.Position.Roofed(turret.Map))
                return false;

            return true;
        }
        
        public static bool CanMan(CompMannable mannable, Pawn pawn)
        {
            // Hard coded mechanoid check :(
            if (!pawn.RaceProps.Humanlike)
                return false;
            
            if (!pawn.RaceProps.ToolUser)
                return false;
            
            if (!pawn.CanReserveAndReach(mannable.parent, PathEndMode.InteractionCell, Danger.Deadly))
                return false;

            var workType = mannable.Props.manWorkType;
            if (workType != WorkTags.None && pawn.WorkTagIsDisabled(workType)) 
                return false;
                
            return true;
        }

        public static void CommandMan(CompMannable mannable, Pawn pawn)
        {
            var job = JobMaker.MakeJob(JobDefOf.ManTurret, mannable.parent);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }
}