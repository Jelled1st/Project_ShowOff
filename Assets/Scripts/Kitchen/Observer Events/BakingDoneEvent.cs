using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingDoneEvent : AObserverEvent
{
    public readonly FryingPan pan;
    public readonly BakableFood food;

    public BakingDoneEvent(FryingPan pan, BakableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}
