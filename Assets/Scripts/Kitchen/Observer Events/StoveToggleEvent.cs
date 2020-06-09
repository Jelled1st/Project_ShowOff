using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveToggleEvent : AObserverEvent
{
    public readonly Stove stove;
    public StoveToggleEvent(Stove stove) : base(stove)
    {
        this.stove = stove;
    }
}
