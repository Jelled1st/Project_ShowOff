using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrownState", menuName = "FarmPlotStates/RoughState", order = 0)]
public class FarmPlotRoughState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._plot = plot;
        this._state = FarmPlot.State.Rough;
        plot.ClearPlants();
        plot.EnableDirtMounds(_enabledDirtMounds);
    }

    public override void Update()
    {
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Dug) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }
}
