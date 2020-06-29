using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoughFarmPlot", menuName = "ScriptableObject/FarmPlotStates/RoughState")]
public class FarmPlotPlantedState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Planted;
        base.EnterState(plot);
    }
    public override FarmPlot.StateReady ReadyForState(FarmPlotState state)
    {
        if (state.GetState() == FarmPlot.State.Growing) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
