using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStirEvent : AObserverEvent
{
    public readonly CookingPan pan;
    public readonly CookableFood food;
    public CookingStirEvent(CookingPan pan, CookableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}
