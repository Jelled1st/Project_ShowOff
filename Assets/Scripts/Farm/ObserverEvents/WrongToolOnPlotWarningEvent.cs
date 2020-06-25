public class WrongToolOnPlotWarningEvent : AObserverEvent
{
    public readonly FarmPlot farmPlot;
    public readonly FarmTool farmTool;

    public WrongToolOnPlotWarningEvent(FarmPlot farmPlot, FarmTool farmTool) : base(farmPlot)
    {
        this.farmPlot = farmPlot;
        this.farmTool = farmTool;
    }
}