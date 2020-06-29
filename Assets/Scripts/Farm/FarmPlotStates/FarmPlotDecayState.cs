using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecayState", menuName = "FarmPlotStates/DecayState", order = 7)]
public class FarmPlotDecayState : FarmPlotState
{
    [SerializeField] private float _timeTillWithered = 10.0f;
    private float _decayTime;

    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Decay;
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Healing) return FarmPlot.StateReady.Ready;
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

    public float GetDecayedTime()
    {
        return _decayTime;
    }

    public override bool SetStateProgress(ProgressBar progressBar)
    {
        progressBar.SetFillColor(new Color(209 / 255.0f, 69 / 255.0f, 69 / 255.0f));
        progressBar.SetPercentage(1 - _decayTime / _timeTillWithered);
        return false;
    }

    public override void ReLoad(FarmPlotState unloadedState)
    {
        if (unloadedState.GetState() == FarmPlot.State.Healing) _plot.PopState();
    }

    public override void ExitState()
    {
    }
}
