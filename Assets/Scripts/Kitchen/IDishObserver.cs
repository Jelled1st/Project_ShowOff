public interface IDishObserver : IObserver
{
    void OnIngredientAdd(ISubject subject, IIngredient ingredient);
    void OnFinishDish(ISubject subject);
}