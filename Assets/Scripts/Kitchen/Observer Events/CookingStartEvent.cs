using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStartEvent : AObserverEvent
{
    public readonly CookingPan pan;
    public readonly CookableFood food;
    public CookingStartEvent(CookingPan pan, CookableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}
