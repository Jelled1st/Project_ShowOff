public class FryerStartEvent : AObserverEvent
{
    public readonly FryFryer fryer;
    public readonly FryableFood food;

    public FryerStartEvent(FryFryer fryer, FryableFood food) : base(fryer)
    {
        this.fryer = fryer;
        this.food = food;
    }
}
