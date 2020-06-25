public class ToolOnCooldownWarningEvent : AObserverEvent
{
    public readonly FarmTool farmTool;

    public ToolOnCooldownWarningEvent(FarmTool farmTool) : base(farmTool)
    {
        this.farmTool = farmTool;
    }
}