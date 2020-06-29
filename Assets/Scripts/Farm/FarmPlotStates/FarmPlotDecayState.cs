using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlotDecayState : FarmPlotState
{
    [SerializeField] private float _timeTillWithered = 10.0f;
    private float _decayTime;

    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Decay;
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlotState state)
    {
        if (state.GetState() == FarmPlot.State.Healing) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
        _decayTime += Time.deltaTime;
        if (_decayTime >= _timeTillWithered)
        {
            _plot.SetState(FarmPlot.State.Withered);
        }
    }
}
