using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StirDishRequiredStir", menuName = "ScriptableObjects/StirDishRequiredStir", order = 1)]
public class StirDishRequiredStir : ScriptableObject, IDishObserver
{
    [SerializeField] List<IngredientType> _stirAfterIngredients;
    private StirDish dish;

    public void Init(StirDish dish)
    {
        this.dish = dish;
        Subscribe(dish);
    }

    public void OnDestroy()
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
        if (_stirAfterIngredients.Count == 0) dish.ReachRequiredStir(this);
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
