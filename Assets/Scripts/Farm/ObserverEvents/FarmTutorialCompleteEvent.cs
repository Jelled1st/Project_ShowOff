using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTutorialCompleteEvent : AObserverEvent
{
    public readonly FarmTutorial farmTutorial;
    public FarmTutorialCompleteEvent(FarmTutorial invoker) : base(invoker)
    {
        this.farmTutorial = invoker;
    }
}
