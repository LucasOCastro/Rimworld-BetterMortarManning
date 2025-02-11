using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterMortarManning;

public class Command_VerbAreaTarget : Command_VerbTarget
{
    public override void ProcessGroupInput(Event ev, List<Gizmo> group)
    {
        var verbs = group.OfType<Command_VerbAreaTarget>().ToList();
        Find.Targeter.BeginTargeting(verb.targetParams, a =>
        {
            Find.Targeter.BeginTargeting(
                verb.targetParams,
                action: b => ConfirmArea(a.Cell, b.Cell, verbs),
                highlightAction: b => HighlightTargets(a.Cell, b.Cell, verbs),
                targetValidator: _ => true,
                onGuiAction: b => DrawArea(a.Cell, b.Cell)
            );
        });
    }

    private static void DrawArea(IntVec3 from, IntVec3 to)
    {
        //TODO draw box in gui maybe
    }

    private static void ConfirmArea(IntVec3 from, IntVec3 to, List<Command_VerbAreaTarget> group)
    {
        var targetCells = GetTargetCells(from, to, group).ToList();
        for (int i = 0; i < group.Count; i++)
        {
            var targetCell = targetCells[i];
            var verb = group[i].verb;
            var turret = (Building_Turret)verb.caster;
            turret.OrderAttack(new(targetCell));
        }
    }

    private static void HighlightTargets(IntVec3 from, IntVec3 to, List<Command_VerbAreaTarget> group)
    {
        var targetCells = GetTargetCells(from, to, group).ToList();
        for (int i = 0; i < group.Count; i++)
        {
            var verb = group[i].verb;
            verb.DrawHighlight(targetCells[i]);
        }
    }

    private static IEnumerable<IntVec3> GetTargetCells(IntVec3 from, IntVec3 to, List<Command_VerbAreaTarget> group)
    {
        var rect = RectUtility.FromTo(from, to);
        return rect.GetGridCells(group.Count);
    }
    
    public override void ProcessInput(Event ev)
    {
        // Is actually called, so we just must inhibit base behavior.
        //throw new InvalidOperationException($"{nameof(ProcessInput)} should not be executed for {nameof(Command_VerbAreaTarget)}"); 
    }
}