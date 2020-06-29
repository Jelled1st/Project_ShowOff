using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlotGrownState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Grown;
        base.EnterState(plot);
    }
    public override FarmPlot.StateReady ReadyForState(FarmPlotState state)
    {
        if (state.GetState() == FarmPlot.State.Rough) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
