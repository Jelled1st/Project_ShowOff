using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFarmPlotObserver : IObserver
{
    void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot);
    void OnPlotHarvest(FarmPlot plot);
}
