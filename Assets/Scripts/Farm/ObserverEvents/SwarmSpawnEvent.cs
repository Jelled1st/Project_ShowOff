using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSpawnEvent : AObserverEvent
{
    public readonly Swarm swarm;
    public SwarmSpawnEvent(Swarm swarm, ISubject invoker) : base(invoker)
    {
        this.swarm = swarm;
    }
}
