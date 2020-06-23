using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDishDraggedToMain : AObserverEvent
{
    public readonly Dish mainDish;
    public readonly Dish sideDish;

    public SideDishDraggedToMain(Dish dish, Dish sideDish) : base(dish)
    {
        this.mainDish = dish;
        this.sideDish = sideDish;
    }
}
