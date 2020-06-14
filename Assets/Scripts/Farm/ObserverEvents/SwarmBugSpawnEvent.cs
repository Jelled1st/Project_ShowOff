using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmBugSpawnEvent : AObserverEvent
{
    public readonly SwarmUnit bug;
    public readonly Swarm swarm;
    public SwarmBugSpawnEvent(Swarm swarm, SwarmUnit bug) : base(swarm)
    {
        this.swarm = swarm;
        this.bug = bug;
    }
}
