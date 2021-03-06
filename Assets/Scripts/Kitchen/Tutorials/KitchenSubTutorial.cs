﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class KitchenSubTutorial : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _tutorialElements;

    [SerializeField]
    private TextMeshProUGUI _friesInFryer;

    [SerializeField]
    private TextMeshProUGUI _meatToFryingPan;

    [SerializeField]
    private TextMeshProUGUI _flipMeat;

    [SerializeField]
    private TextMeshProUGUI _dragIngredientToCuttingBoard;

    [SerializeField]
    private TextMeshProUGUI _cutIngredient;

    [SerializeField]
    private Dish _sideDish;

    [SerializeField]
    private UnityEvent _onExecute;

    private bool _firstFryer = false;

    [SerializeField]
    private UnityEvent _onFryingStart;

    private bool _firstFryDone = false;

    [SerializeField]
    protected UnityEvent _onFryingDone;

    private bool _firstCook = false;

    [SerializeField]
    protected UnityEvent _onCookingStart;

    private bool _firstCookDone = false;

    [SerializeField]
    protected UnityEvent _onCookingDone;

    private bool _firstBaking = false;

    [SerializeField]
    protected UnityEvent _onBakingStart;

    private bool _firstSide = false;

    [SerializeField]
    protected UnityEvent _onSideBaked;

    private bool _firstBakingFlip = false;

    [SerializeField]
    protected UnityEvent _onBakingFlip;

    private bool _firstBakingDone = false;

    [SerializeField]
    protected UnityEvent _onBakingDone;

    private bool _firstIngredientCut = false;

    [SerializeField]
    protected UnityEvent _onIngredientCut;

    private bool _firstIngredientCutUp = false;

    [SerializeField]
    protected UnityEvent _onIngredientCutUp;

    private bool _firstIngredientOnBoard = false;

    [SerializeField]
    protected UnityEvent _onIngredientToCuttingBoard;

    private bool _firstIngredientPulled = false;

    [SerializeField]
    protected UnityEvent _onIngredientPulled;

    private bool _firstIngredientAdded = false;

    [SerializeField]
    protected UnityEvent _onIngredientToDish;

    [SerializeField]
    protected UnityEvent _onFirstIngredientToDish;

    private bool _firstIngredientSideDish = false;

    [SerializeField]
    protected UnityEvent _onIngredientToSideDish;

    private bool _firstCookingStir = false;

    [SerializeField]
    protected UnityEvent _onCookingStir;

    private bool _firstDishStir = false;

    [SerializeField]
    protected UnityEvent _onDishStir;

    private bool _firstCookingAllDone = false;

    [SerializeField]
    protected UnityEvent _onCookingAllIngredientsDone;

    [SerializeField]
    protected UnityEvent _onDishDone;

    [SerializeField]
    protected UnityEvent _onSideDishToMain;

    public void DisableAllElements()
    {
        for (var i = 0; i < _tutorialElements.Count; ++i)
        {
            _tutorialElements[i].SetActive(false);
        }
    }

    public void EnableAllElements()
    {
        for (var i = 0; i < _tutorialElements.Count; ++i)
        {
            _tutorialElements[i].SetActive(true);
        }
    }

    public static void StrikeThroughText(TextMeshProUGUI text)
    {
        if (text != null) text.fontStyle = FontStyles.Strikethrough;
    }

    public void EnableStrikeThrough(TextMeshProUGUI text)
    {
        StrikeThroughText(text);
    }

    public virtual void Execute()
    {
        _onExecute.Invoke();
    }

    public virtual void FryingStart()
    {
        if (_firstFryer) return;
        _firstFryer = true;

        _onFryingStart.Invoke();
        StrikeThroughText(_friesInFryer);
    }

    public virtual void FryingDone()
    {
        if (_firstFryDone) return;
        _firstFryer = true;

        _onFryingDone.Invoke();
    }

    public virtual void CookingStart()
    {
        if (_firstCook) return;
        _firstCook = true;

        _onCookingStart.Invoke();
    }

    public virtual void RepeatCookingStart()
    {
        _firstCook = false;
    }

    public virtual void CookingDone()
    {
        if (_firstCookDone) return;
        _firstCookDone = true;

        _onCookingDone.Invoke();
    }

    public virtual void CookingAllIngredientsDone()
    {
        if (_firstCookingAllDone) return;
        _firstCookingAllDone = true;

        _onCookingAllIngredientsDone.Invoke();
    }

    public virtual void BakingStart()
    {
        if (_firstBaking) return;
        _firstBaking = true;

        _onBakingStart.Invoke();
        StrikeThroughText(_meatToFryingPan);
    }

    public virtual void SideBakedDone()
    {
        if (_firstSide) return;
        _firstSide = true;

        _onSideBaked.Invoke();
    }

    public virtual void BakingFlip()
    {
        if (_firstBakingFlip || !_firstSide) return;
        _firstBakingFlip = true;

        _onBakingFlip.Invoke();
        StrikeThroughText(_flipMeat);
    }

    public void RepeatCookingStir()
    {
        _firstCookingStir = false;
    }

    public void CookingStir()
    {
        if (_firstCookingStir) return;
        _firstCookingStir = true;

        _onCookingStir.Invoke();
    }

    public void DishStir()
    {
        if (_firstDishStir) return;
        _firstDishStir = true;

        _onDishStir.Invoke();
    }

    public virtual void BakingDone()
    {
        if (_firstBakingDone) return;
        _firstBakingDone = true;

        _onBakingDone.Invoke();
    }

    public virtual void IngredientCut()
    {
        if (_firstIngredientCut) return;
        _firstIngredientCut = true;

        _onIngredientCut.Invoke();
        StrikeThroughText(_cutIngredient);
    }

    public virtual void IngredientCutUp()
    {
        if (_firstIngredientCutUp) return;
        _firstIngredientCutUp = true;

        _onIngredientCutUp.Invoke();
    }

    public virtual void IngredientToCuttingBoard()
    {
        if (_firstIngredientOnBoard) return;
        _firstIngredientOnBoard = true;

        _onIngredientToCuttingBoard.Invoke();
        StrikeThroughText(_dragIngredientToCuttingBoard);
    }

    public virtual void IngredientPulled()
    {
        if (_firstIngredientPulled) return;
        _firstIngredientPulled = true;

        _onIngredientPulled.Invoke();
    }

    public virtual void IngredientAddedToDish(Dish dish)
    {
        _onIngredientToDish.Invoke();
        if (dish == _sideDish && !_firstIngredientSideDish)
        {
            _firstIngredientSideDish = true;
            _onIngredientToSideDish.Invoke();
        }

        if (_firstIngredientAdded) return;
        _firstIngredientAdded = true;
        _onFirstIngredientToDish.Invoke();
    }

    public virtual void IngredientDone(IIngredient ingredient)
    {
    }

    public virtual void DishComplete(Dish dish)
    {
        _onDishDone.Invoke();
    }

    public virtual void SideDishToMain()
    {
        _onSideDishToMain.Invoke();
    }
}