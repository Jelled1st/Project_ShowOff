using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StirEvent : AObserverEvent
{
    public readonly CookingPan pan;
    public readonly CookableFood food;
    public StirEvent(CookingPan pan, CookableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}
