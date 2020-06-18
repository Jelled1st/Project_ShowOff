using UnityEngine.Events;

[System.Serializable]
public struct PlotChangeUnityEvent
{
    public FarmPlot.State newState;
    public FarmPlot.State previousState;
    public UnityEvent events;

    public void TryInvoke(FarmPlot.State state, FarmPlot.State preState)
    {
        if(newState == state && previousState == preState)
        {
            events.Invoke();
        }
    }
}
