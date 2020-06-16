using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBakedEvent : AObserverEvent
{
    public readonly BakableFood food;
    public readonly int side;
    public SideBakedEvent(BakableFood food, int side) : base(food)
    {
        this.food = food;
        this.side = side;
    }
}
