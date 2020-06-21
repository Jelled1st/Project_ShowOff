public class BakingStartBurnEvent : AObserverEvent
{
    public readonly BakableFood food;

    public BakingStartBurnEvent(BakableFood food) : base(food)
    {
        this.food = food;
    }
}
