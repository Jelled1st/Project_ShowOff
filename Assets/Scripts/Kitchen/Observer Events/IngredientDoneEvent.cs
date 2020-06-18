public class IngredientDoneEvent : AObserverEvent
{
    public readonly IIngredient ingredient;
    public IngredientDoneEvent(IIngredient ingredient) : base(ingredient)
    {
        this.ingredient = ingredient;
    }
}
