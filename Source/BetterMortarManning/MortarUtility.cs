using RimWorld;
using Verse;
using Verse.AI;

namespace BetterMortarManning
{
    public static class MortarUtility
    {
        public static bool CanMan(CompMannable mannable, Pawn pawn)
        {
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