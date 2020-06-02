using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Pickle,
    Cheese,
    Lettuce,
    Patty,
    Fries,
    Buns,
    Apple,
}

public interface IIngredient
{
    IngredientType GetIngredientType();
    bool ReadyForDish();
    void AddedToDish();
    float GetHeight();
    GameObject GetDishMesh();
}
