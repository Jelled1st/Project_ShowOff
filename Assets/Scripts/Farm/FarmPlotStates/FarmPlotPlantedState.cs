using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantedState", menuName = "FarmPlotStates/PlantedState", order = 4)]
public class FarmPlotPlantedState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Planted;
        base.EnterState(plot);
    }
    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Growing) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
