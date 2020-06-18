public class FarmPlotIsInactiveWarningEvent : AObserverEvent
{
    public readonly FarmPlot farmPlot;

    public FarmPlotIsInactiveWarningEvent(FarmPlot farmPlot) : base(farmPlot)
    {
        this.farmPlot = farmPlot;
    }
}
