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
    Carrot,
    CayennePepper,
    ChiliPowder,
    DicedChicken,
    GroundTurmeric,
    SeaSalt,
    RawFish,
    Cucumber,
    FetaCheese,
    LettuceLeaves,
    Paprika, 
}

public interface IIngredient : ISubject
{
    IngredientType GetIngredientType();
    bool ReadyForDish();
    void AddedToDish();
    float GetHeight();
    GameObject GetDishMesh();
}
