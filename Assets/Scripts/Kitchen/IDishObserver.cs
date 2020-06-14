using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDishObserver : IObserver
{
    void OnIngredientAdd(ISubject subject, IIngredient ingredient);
    void OnFinishDish(ISubject subject);

}
