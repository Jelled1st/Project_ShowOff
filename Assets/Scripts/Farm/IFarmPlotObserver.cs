public interface IFarmPlotObserver : IObserver
{
    void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot);
    void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot);
    void OnPlotHarvest(FarmPlot plot);
}
