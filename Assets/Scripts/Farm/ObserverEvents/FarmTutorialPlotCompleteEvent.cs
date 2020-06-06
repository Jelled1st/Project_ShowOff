using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTutorialPlotCompleteEvent : AObserverEvent
{
    public readonly FarmTutorial farmTutorial;
    public FarmTutorialPlotCompleteEvent(FarmTutorial invoker) : base(invoker)
    {
        this.farmTutorial = invoker;
    }
}
