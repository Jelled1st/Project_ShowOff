using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmBugSpawnEvent : AObserverEvent
{
    public readonly SwarmUnit bug;
    public SwarmBugSpawnEvent(ISubject invoker, SwarmUnit bug) : base(invoker)
    {
        this.bug = bug;
    }
}
