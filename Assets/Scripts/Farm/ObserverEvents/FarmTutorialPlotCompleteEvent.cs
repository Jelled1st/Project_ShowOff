﻿public class FarmTutorialPlotCompleteEvent : AObserverEvent
{
    public readonly FarmTutorial farmTutorial;

    public FarmTutorialPlotCompleteEvent(FarmTutorial invoker) : base(invoker)
    {
        farmTutorial = invoker;
    }
}