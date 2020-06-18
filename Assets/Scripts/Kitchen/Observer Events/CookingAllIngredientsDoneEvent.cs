using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingAllIngredientsDoneEvent : AObserverEvent
{
    public readonly CookingPan pan;
    public CookingAllIngredientsDoneEvent(CookingPan pan) : base(pan)
    {
        this.pan = pan;
    }
}
