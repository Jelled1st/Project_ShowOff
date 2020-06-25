public class DishStirEvent : AObserverEvent
{
    public readonly Dish dish;

    public DishStirEvent(Dish dish) : base(dish)
    {
        this.dish = dish;
    }
}