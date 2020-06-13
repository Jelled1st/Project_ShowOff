using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StirDish : Dish
{
    [Header("Stir dish exlusive")]
    [SerializeField] GameObject _stirringDevice;
    [SerializeField] List<IngredientType> _requiredStirAfterIngredient;
    private bool _mustStir = false;
    bool _isFinished = false;

    new void Awake() => base.Awake();
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override bool IsFinished(bool includeFinishIngredient)
    {
        return base.IsFinished(includeFinishIngredient) && _mustStir == false;
    }

    protected override bool TryAddIngredient(IIngredient ingredient)
    {
        bool setMustStir = false;
        int stirIngredientIndex = -1;
        for(int i = 0; i < _requiredStirAfterIngredient.Count; ++i)
        {
            if(ingredient.GetIngredientType() == _requiredStirAfterIngredient[i])
            {
                stirIngredientIndex = i;
                setMustStir = true;
                break;
            }
        }
        if (TryAddIngredientCheck(ingredient))
        {
            if (setMustStir)
            {
                _mustStir = true;
                _requiredStirAfterIngredient.RemoveAt(stirIngredientIndex);
            }
            else if(IsFinished(true) && !_isFinished)
            {
                _isFinished = true;
                InformObserversFinish();
            }
            return true;
        }
        else return false;
    }

    private bool TryAddIngredientCheck(IIngredient ingredient)
    {
        if (_debugLog) Debug.Log("Trying to add");
        IngredientType type = ingredient.GetIngredientType();
        if (_finishIngredient != IngredientType.Undefined && type == _finishIngredient)
        {
            if (IsFinished(false))
            {
                if (_debugLog) Debug.Log("Finish ingredient");
                //required ingredient
                AddFinalIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight());
                InformObserversAddIngredient(ingredient);
                _finishIngredientPlaced = true;
                InformObserversFinish();
                return true;
            }
        }
        if (_finishIngredientPlaced) return false;
        for (int i = 0; i < _requiredIngredients.Count; ++i)
        {
            if (type == _requiredIngredients[i])
            {
                if (_debugLog) Debug.Log("Required ingredient");
                //required ingredient
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight(), true, i);
                _requiredIngredients.RemoveAt(i); //remove ingredient from the list
                InformObserversAddIngredient(ingredient);
                return true;
            }
        }
        for (int i = 0; i < _optionalIngredients.Count; ++i)
        {
            if (type == _optionalIngredients[i])
            {
                if (_debugLog) Debug.Log("Optional ingredient");
                //required ingredient
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight(), false, i);
                _optionalIngredients.RemoveAt(i); //remove ingredient from the list
                InformObserversAddIngredient(ingredient);
                return true;
            }
        }
        if (_debugLog) Debug.Log("Could not be added");
        return false;
    }

    public override void OnHold(float holdTime, Vector3 hitPoint)
    {
        base.OnHold(holdTime, hitPoint);

        if (!DOTween.IsTweening(_stirringDevice.transform)) _stirringDevice.transform.DORotate(new Vector3(0, 360, 0), 0.7f, RotateMode.LocalAxisAdd);
        Notify(new DishStirEvent(this));
        _mustStir = false;
        if (IsFinished(true) && !_isFinished)
        {
            _isFinished = true;
            InformObserversFinish();
        }
    }
}
