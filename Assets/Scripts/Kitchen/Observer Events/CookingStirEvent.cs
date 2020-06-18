public class CookingStirEvent : AObserverEvent
{
    public readonly CookingPan pan;
    public CookingStirEvent(CookingPan pan) : base(pan)
    {
        this.pan = pan;
    }
}
