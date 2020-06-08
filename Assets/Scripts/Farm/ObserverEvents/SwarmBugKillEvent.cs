using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmBugKillEvent : AObserverEvent
{
    public readonly SwarmUnit bug;
    public readonly Swarm swarm;
    public SwarmBugKillEvent(SwarmUnit swarmUnit, Swarm invoker) : base(invoker)
    {
        this.bug = swarmUnit;
        this.swarm = invoker;
    }
}
