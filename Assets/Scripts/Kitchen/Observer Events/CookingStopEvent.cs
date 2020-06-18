public class CookingStopEvent : AObserverEvent
{
    public readonly CookingPan pan;
    public readonly CookableFood food;
    public CookingStopEvent(CookingPan pan, CookableFood food) : base(pan)
    {
        this.pan = pan;
        this.food = food;
    }
}
