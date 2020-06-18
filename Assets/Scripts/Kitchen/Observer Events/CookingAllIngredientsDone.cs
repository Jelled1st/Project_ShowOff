using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingAllIngredientsDone : AObserverEvent
{
    public readonly CookingPan pan;
    public CookingAllIngredientsDone(CookingPan pan) : base(pan)
    {
        this.pan = pan;
    }
}
