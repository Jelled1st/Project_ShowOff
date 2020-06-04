﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DishIngredientAddUnityEvent
{
    public IngredientType ingredientType;
    public UnityEvent events;

    public void TryInvoke(IngredientType ingredient)
    {
        if(ingredient == ingredientType)
        {
            events.Invoke();
        }
    }
}