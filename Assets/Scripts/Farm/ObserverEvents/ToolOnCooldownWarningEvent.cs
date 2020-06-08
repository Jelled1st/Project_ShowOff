using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolOnCooldownWarningEvent : AObserverEvent
{
    public readonly FarmTool farmTool;
    public ToolOnCooldownWarningEvent(FarmTool farmTool) : base(farmTool)
    {
        this.farmTool = farmTool;
    }
}
