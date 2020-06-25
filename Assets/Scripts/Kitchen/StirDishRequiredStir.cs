using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StirDishRequiredStir", menuName = "ScriptableObjects/StirDishRequiredStir", order = 1)]
public class StirDishRequiredStir : ScriptableObject, IDishObserver
{
    [SerializeField]
    private List<IngredientType> _stirAfterIngredients;

    private List<IngredientType> _uniqueTypes;
    private List<IngredientType> _ingredients;
    private StirDish dish;

    public void Init(StirDish dish)
    {
        if (dish != null) UnSubscribe(dish);
        this.dish = dish;
        Subscribe(dish);

        _uniqueTypes = new List<IngredientType>();
        for (var i = 0; i < _stirAfterIngredients.Count; ++i)
        {
            if (!_uniqueTypes.Contains(_stirAfterIngredients[i])) _uniqueTypes.Add(_stirAfterIngredients[i]);
        }

        _ingredients = new List<IngredientType>(_stirAfterIngredients);
    }

    public bool HasIngredientType(IngredientType type)
    {
        return _uniqueTypes.Contains(type);
    }

    public void RemoveDish()
    {
        UnSubscribe(dish);
    }

    public void OnFinishDish(ISubject subject)
    {
    }

    public void OnIngredientAdd(ISubject subject, IIngredient ingredient)
    {
        for (var i = 0; i < _ingredients.Count; ++i)
        {
            if (_ingredients[i] == ingredient.GetIngredientType())
            {
                _ingredients.RemoveAt(i);
                break;
            }
        }

        if (_ingredients.Count == 0)
        {
            dish.ReachRequiredStir(this);
        }
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }
}