using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingBurntEvent : AObserverEvent
{
    public readonly FryingPan pan;
    public readonly BakableFood food;

    public BakingBurntEvent(FryingPan pan, BakableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}
