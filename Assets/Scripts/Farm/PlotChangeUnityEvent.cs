using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct PlotChangeUnityEvent
{
    public FarmPlot.State newState;
    public FarmPlot.State previousState;
    public UnityEvent events;
}
