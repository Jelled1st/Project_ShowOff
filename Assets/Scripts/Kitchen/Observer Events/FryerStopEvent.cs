public class FryerStopEvent : AObserverEvent
{
    public readonly FryFryer fryer;
    public readonly FryableFood food;

    public FryerStopEvent(FryFryer fryer, FryableFood food) : base(fryer)
    {
        this.fryer = fryer;
        this.food = food;
    }
}