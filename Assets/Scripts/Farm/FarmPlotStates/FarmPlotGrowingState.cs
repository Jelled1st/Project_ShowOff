using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlotGrowingState : FarmPlotState
{
    [SerializeField] private float _timeTillGrown = 10.0f;
    private float _growTime = 0; 

    public override void EnterState(FarmPlot plot)
    {
        base.EnterState(plot);
    }

    public override FarmPlot.StateReady ReadyForState(FarmPlotState state)
    {
        if (state.GetState() == FarmPlot.State.Decay) return FarmPlot.StateReady.Ready;
        else return FarmPlot.StateReady.InvalidAdvancement;
    }

    public override void Update()
    {
        _growTime += Time.deltaTime;
        if (_growTime >= _timeTillGrown) _plot.SetState(FarmPlot.State.Grown);
    }

    public override bool SetStateProgress(ProgressBar progressBar)
    {
        progressBar.SetFillColor(new Color(102 / 255.0f, 77 / 255.0f, 63 / 255.0f));
        float percentage = _growTime / _timeTillGrown;
        if (percentage <= 1.0f) progressBar.SetPercentage(percentage);
        else progressBar.SetPercentage(1.0f);
        return true;
    }
}
