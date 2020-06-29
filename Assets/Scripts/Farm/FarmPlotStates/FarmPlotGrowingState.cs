using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrowingState", menuName = "FarmPlotStates/GrowingState", order = 5)]
public class FarmPlotGrowingState : FarmPlotState
{
    [SerializeField] private float _timeTillGrown = 10.0f;
    [SerializeField] private float _decayGrowSpeed;
    private float _growTime = 0;

    public override void EnterState(FarmPlot plot)
    {
        this._state = FarmPlot.State.Growing;
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlot.State state)
    {
        if (state == FarmPlot.State.Decay) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
        _growTime += Time.deltaTime;
        if (_growTime >= _timeTillGrown) _plot.SetState(FarmPlot.State.Grown);
    }

    public override void ReLoad(FarmPlotState unloadedState)
    {
        _plot.ClearPlants();
        _plot.SetPlants(_plants);
        if(unloadedState.GetState() == FarmPlot.State.Decay)
        {
            FarmPlotDecayState decayState = unloadedState as FarmPlotDecayState;
            _growTime = Mathf.Min(_growTime + decayState.GetDecayedTime() * _decayGrowSpeed, _timeTillGrown - 1.0f);
            Destroy(decayState);
        }
    }

    public override bool SetStateProgress(ProgressBar progressBar)
    {
        progressBar.SetFillColor(new Color(102 / 255.0f, 77 / 255.0f, 63 / 255.0f));
        float percentage = _growTime / _timeTillGrown;
        if (percentage < 1.0f) progressBar.SetPercentage(percentage);
        else
        {
            progressBar.SetPercentage(1.0f);
            return true;
        }
        return false;
    }
}
