﻿public class FarmTutorialCompleteEvent : AObserverEvent
{
    public readonly FarmTutorial farmTutorial;

    public FarmTutorialCompleteEvent(FarmTutorial invoker) : base(invoker)
    {
        farmTutorial = invoker;
    }
}