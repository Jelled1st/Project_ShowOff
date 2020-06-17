using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StirDishRequiredStir", menuName = "ScriptableObjects/StirDishRequiredStir", order = 1)]
public class StirDishRequiredStir : ScriptableObject, IDishObserver
{
    [SerializeField] List<IngredientType> _stirAfterIngredients;
    List<IngredientType> _uniqueTypes;
    private StirDish dish;

    public void Init(StirDish dish)
    {
        if (dish != null) UnSubscribe(dish);
        this.dish = dish;
        Subscribe(dish);

        _uniqueTypes = new List<IngredientType>();
        for (int i = 0; i < _stirAfterIngredients.Count; ++i)
        {
            if (!_uniqueTypes.Contains(_stirAfterIngredients[i])) _uniqueTypes.Add(_stirAfterIngredients[i]);
        }
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
        for(int i = 0; i < _stirAfterIngredients.Count; ++i)
        {
            if(_stirAfterIngredients[i] == ingredient.GetIngredientType())
            {
                _stirAfterIngredients.RemoveAt(i);
                break;
            }
        }
        if (_stirAfterIngredients.Count == 0)
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
