using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WithererState", menuName = "FarmPlotStates/WitheredState", order = 9)]
public class FarmPlotWitheredState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Withered;
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Dug) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
