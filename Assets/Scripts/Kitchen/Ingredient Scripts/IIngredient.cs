using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Undefined,
    Pickle,
    Cheese,
    Lettuce,
    Patty,
    Fries,
    Buns,
    Apple,
    Beans,
    Tomato,
    Onion,
    ChickenFilet,
    ShreddedCheese,
}

public interface IIngredient
{
    IngredientType GetIngredientType();
    bool ReadyForDish();
    void AddedToDish();
    float GetHeight();
    GameObject GetDishMesh();
}
