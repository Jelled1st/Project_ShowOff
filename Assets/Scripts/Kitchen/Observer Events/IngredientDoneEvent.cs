using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDoneEvent : AObserverEvent
{
    public readonly IIngredient ingredient;
    public IngredientDoneEvent(IIngredient ingredient) : base(ingredient)
    {
        this.ingredient = ingredient;
    }
}
