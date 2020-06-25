public class SwarmBugKillEvent : AObserverEvent
{
    public readonly SwarmUnit bug;
    public readonly Swarm swarm;

    public SwarmBugKillEvent(SwarmUnit swarmUnit, Swarm invoker) : base(invoker)
    {
        bug = swarmUnit;
        swarm = invoker;
    }
}