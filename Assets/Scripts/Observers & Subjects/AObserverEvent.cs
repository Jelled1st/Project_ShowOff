using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AObserverEvent
{
    public readonly ISubject invoker;

    public AObserverEvent(ISubject invoker)
    {
        this.invoker = invoker;
    }
}
