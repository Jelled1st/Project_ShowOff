public abstract class AObserverEvent
{
    public readonly ISubject invoker;

    public AObserverEvent(ISubject invoker)
    {
        this.invoker = invoker;
    }
}