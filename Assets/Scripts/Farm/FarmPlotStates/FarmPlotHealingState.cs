using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealingState", menuName = "FarmPlotStates/HealingState", order = 8)]
public class FarmPlotHealingState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Healing;
        this._plot = plot;
        plot.PopStateAfterCooldown();
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
