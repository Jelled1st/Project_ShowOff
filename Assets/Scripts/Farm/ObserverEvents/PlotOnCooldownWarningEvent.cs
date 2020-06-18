public class PlotOnCooldownWarningEvent : AObserverEvent
{
    public readonly FarmPlot farmPlot;
    public readonly FarmTool farmTool;

    public PlotOnCooldownWarningEvent(FarmPlot farmPlot, FarmTool farmTool) : base(farmPlot)
    {
        this.farmPlot = farmPlot;
        this.farmTool = farmTool;
    }
}
