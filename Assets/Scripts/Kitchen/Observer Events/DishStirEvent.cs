using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishStirEvent : AObserverEvent
{
    public readonly Dish dish;

    public DishStirEvent(Dish dish) : base(dish)
    {
        this.dish = dish;
    }
}
