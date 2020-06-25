public class BakingStartEvent : AObserverEvent
{
    public readonly FryingPan pan;
    public readonly BakableFood food;

    public BakingStartEvent(FryingPan pan, BakableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}