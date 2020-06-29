using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoughFarmPlot", menuName = "ScriptableObject/FarmPlotStates/RoughState")]
public class FarmPlotRoughState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this.plot = plot;
        plot.ClearPlants();
        plot.EnableDirtMounds(_enabledDirtMounds);
    }

    public override void Update()
    {
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlotState state)
    {
        if (state.GetState() == FarmPlot.State.Dug) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }
}
