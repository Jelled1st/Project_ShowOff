using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrownState", menuName = "FarmPlotStates/GrownState", order = 6)]
public class FarmPlotGrownState : FarmPlotState
{
    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Grown;
        base.EnterState(plot);
    }
    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Rough || state == FarmPlot.State.Harvested) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
    }
}
