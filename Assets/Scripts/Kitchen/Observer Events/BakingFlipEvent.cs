using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingFlipEvent : AObserverEvent
{
    public readonly BakableFood food;
    public BakingFlipEvent(BakableFood food) : base(food)
    {
        this.food = food;
    }
}
