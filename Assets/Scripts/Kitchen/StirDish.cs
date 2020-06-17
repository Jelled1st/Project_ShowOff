using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StirDish : Dish
{
    [Header("Stir dish exlusive")]
    [SerializeField] GameObject _stirringDevice;
    [SerializeField] List<StirDishRequiredStir> _requiredStirOptions;
    [SerializeField] GameObject _ingredientRotateParent;
    private bool _mustStir = false;
    bool _isFinished = false;
    private StirDishRequiredStir _currentRequiredStir = null;

    new void Awake() => base.Awake();
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        for(int i = 0; i < _requiredStirOptions.Count; ++i)
        {
            _requiredStirOptions[i].Init(this);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override bool IsFinished(bool includeFinishIngredient)
    {
        return base.IsFinished(includeFinishIngredient) && !_mustStir;
    }

    protected override bool TryAddIngredient(IIngredient ingredient)
    {
        if (TryAddIngredientCheck(ingredient))
        {
            if(IsFinished(true) && !_isFinished)
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
        if (_mustStir && !_currentRequiredStir.HasIngredientType(ingredient.GetIngredientType())) return false;
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

    public void ReachRequiredStir(StirDishRequiredStir requiredStir)
    {
        for(int i = 0; i < _requiredStirOptions.Count; ++i)
        {
            if(_requiredStirOptions[i] == requiredStir)
            {
                SetRequiredStir(true);
                _requiredStirOptions.RemoveAt(i);
                _currentRequiredStir = requiredStir;
                requiredStir.RemoveDish();
            }
        }
    }

    public void SetRequiredStir(bool value)
    {
        _mustStir = value;
    }

    public override GameObject GetDragCopy()
    {
        GameObject copy = base.GetDragCopy();
        if (copy == null) return null;
        Destroy(copy.GetComponent<StirDish>());
        Destroy(copy.GetComponent<Collider>());
        return copy;
    }

    public override void OnPress(Vector3 hitPoint)
    { 
        base.OnPress(hitPoint);

        if (!DOTween.IsTweening(_stirringDevice.transform))
        {
            _stirringDevice.transform.DORotate(new Vector3(0, 360, 0), 0.7f, RotateMode.LocalAxisAdd);
            _ingredientRotateParent.transform.DOLocalRotate(new Vector3(0, 360, 0), 0.7f, RotateMode.WorldAxisAdd);

            Notify(new DishStirEvent(this));
            _mustStir = false;
            _currentRequiredStir = null;
            if (IsFinished(true) && !_isFinished)
            {
                _isFinished = true;
                InformObserversFinish();
            }
        }
    }
}
