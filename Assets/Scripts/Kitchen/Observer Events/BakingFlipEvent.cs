public class BakingFlipEvent : AObserverEvent
{
    public readonly BakableFood food;

    public BakingFlipEvent(BakableFood food) : base(food)
    {
        this.food = food;
    }
}