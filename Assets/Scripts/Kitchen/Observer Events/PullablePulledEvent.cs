using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullablePulledEvent : AObserverEvent
{
    public readonly PullableFood food;
    public PullablePulledEvent(PullableFood food) : base(food)
    {
        this.food = food;
    }
}
