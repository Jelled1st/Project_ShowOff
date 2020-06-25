using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StirDish : Dish
{
    [Header("Stir dish exlusive")]
    [SerializeField]
    private GameObject _spoon;

    [SerializeField]
    private List<StirDishRequiredStir> _requiredStirOptions;

    [SerializeField]
    private GameObject _ingredientRotateParent;

    [SerializeField]
    private float _stirTime = 0.8f;

    private bool _mustStir = false;
    private bool _isFinished = false;
    private StirDishRequiredStir _currentRequiredStir = null;
    private Animator _spoonAnimator;

    private new void Awake()
    {
        base.Awake();
    }

    private new void Start()
    {
        base.Start();
        _spoonAnimator = _spoon.GetComponent<Animator>();
        for (var i = 0; i < _requiredStirOptions.Count; ++i)
        {
            _requiredStirOptions[i].Init(this);
        }
    }

    protected override bool IsFinished(bool includeFinishIngredient)
    {
        return base.IsFinished(includeFinishIngredient) && !_mustStir;
    }

    protected override bool TryAddIngredient(IIngredient ingredient)
    {
        if (TryAddIngredientCheck(ingredient))
        {
            if (IsFinished(true) && !_isFinished)
            {
                _isFinished = true;
                InformObserversFinish();
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TryAddIngredientCheck(IIngredient ingredient)
    {
        if (_mustStir && !_currentRequiredStir.HasIngredientType(ingredient.GetIngredientType())) return false;
        if (_debugLog) Debug.Log("Trying to add");
        var type = ingredient.GetIngredientType();
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
        for (var i = 0; i < _requiredIngredients.Count; ++i)
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

        for (var i = 0; i < _optionalIngredients.Count; ++i)
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
        for (var i = 0; i < _requiredStirOptions.Count; ++i)
        {
            if (_requiredStirOptions[i] == requiredStir)
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
        var copy = base.GetDragCopy();
        if (copy == null) return null;
        Destroy(copy.GetComponent<StirDish>());
        Destroy(copy.GetComponent<Collider>());
        return copy;
    }

    public override void OnPress(Vector3 hitPoint)
    {
        base.OnPress(hitPoint);

        if (!_spoonAnimator.GetCurrentAnimatorStateInfo(0).IsName("anim_spoon_stirring"))
        {
            _spoonAnimator.SetTrigger("isPlaying");
            _ingredientRotateParent.transform.DOLocalRotate(new Vector3(0, 360, 0), _stirTime, RotateMode.WorldAxisAdd);

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