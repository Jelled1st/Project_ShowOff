using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FarmPlotState : ScriptableObject
{
    [SerializeField] protected List<GameObject> _plants;
    [SerializeField] protected bool _enabledDirtMounds;
    protected FarmPlot.State _state;
    protected FarmPlot plot;

    public virtual void EnterState(FarmPlot plot)
    {
        this.plot = plot;
        plot.ClearPlants();
        plot.SetPlants(_plants);
        plot.EnableDirtMounds(_enabledDirtMounds);
    }

    public abstract void Update();

    public virtual void ExitState()
    {
        Destroy(this);
    }

    // Returns true when the progress is done.
    // Returns false when in progrss
    public virtual bool SetStateProgress(ProgressBar progressBar)
    {
        return true;
    }

    public abstract FarmPlot.StateReady ReadyForState(FarmPlotState state);
    public FarmPlot.State GetState()
    {
        return _state;
    }
}
