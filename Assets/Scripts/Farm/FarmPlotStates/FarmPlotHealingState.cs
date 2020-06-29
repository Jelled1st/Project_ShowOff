using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlotHealingState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Healing;
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlotState state)
    {
        return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
