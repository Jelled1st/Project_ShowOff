using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DugState", menuName = "FarmPlotStates/DugState", order = 1)]
public class FarmPlotDugState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Dug;
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Planted) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
