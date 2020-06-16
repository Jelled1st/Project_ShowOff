using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public abstract class KitchenSubTutorial : MonoBehaviour
{
    [SerializeField] List<GameObject> _tutorialElements;
    [SerializeField] TextMeshProUGUI _friesInFryer;
    [SerializeField] TextMeshProUGUI _meatToFryingPan;
    [SerializeField] TextMeshProUGUI _flipMeat;
    [SerializeField] TextMeshProUGUI _dragIngredientToCuttingBoard;
    [SerializeField] TextMeshProUGUI _cutIngredient;
    [SerializeField] UnityEvent _onExecute;
    private bool _firstFryer = false;
    [SerializeField] UnityEvent _onFryingStart;
    private bool _firstFryDone = false;
    [SerializeField] UnityEvent _onFryingDone;
    private bool _firstCook = false;
    [SerializeField] UnityEvent _onCookingStart;
    private bool _firstCookDone = false;
    [SerializeField] UnityEvent _onCookingDone;
    private bool _firstBaking = false;
    [SerializeField] UnityEvent _onBakingStart;
    private bool _firstBakingFlip = true;
    [SerializeField] UnityEvent _onBakingFlip;
    private bool _firstBakingDone = false;
    [SerializeField] UnityEvent _onBakingDone;
    private bool _firstIngredientCut = false;
    [SerializeField] UnityEvent _onIngredientCut;
    private bool _firstIngredientOnBoard = false;
    [SerializeField] UnityEvent _onIngredientToCuttingBoard;
    private bool _firstIngredientPulled = false;
    [SerializeField] UnityEvent _onIngredientPulled;

    public void DisableAllElements()
    {
        for (int i = 0; i < _tutorialElements.Count; ++i)
        {
            _tutorialElements[i].SetActive(false);
        }
    }

    public void EnableAllElements()
    {
        for (int i = 0; i < _tutorialElements.Count; ++i)
        {
            _tutorialElements[i].SetActive(true);
        }
    }

    public virtual void Execute() => _onExecute.Invoke();
    public virtual void FryingStart()
    {
        if (_firstFryer) return;
        _firstFryer = true;

        _onFryingStart.Invoke();
        if(_friesInFryer != null) _friesInFryer.fontStyle = FontStyles.Strikethrough;
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

    public virtual void CookingDone()
    {
        if (_firstCookDone) return;
        _firstCookDone = true;

        _onCookingDone.Invoke();
    }
    public virtual void BakingStart()
    {
        if (_firstBaking) return;
        _firstBaking = true;

        _onBakingStart.Invoke();
        if (_meatToFryingPan != null) _meatToFryingPan.fontStyle = FontStyles.Strikethrough;
    }

    public virtual void BakingFlip()
    {
        if (_firstBakingFlip) return;
        _firstBakingFlip = true;

        _onBakingFlip.Invoke();
        if (_flipMeat != null) _flipMeat.fontStyle = FontStyles.Strikethrough;
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
        if (_cutIngredient != null) _cutIngredient.fontStyle = FontStyles.Strikethrough;
    }
    public virtual void IngredientToCuttingBoard()
    {
        if (_firstIngredientOnBoard) return;
        _firstIngredientOnBoard = true;

        _onIngredientToCuttingBoard.Invoke();
        if (_dragIngredientToCuttingBoard != null) _dragIngredientToCuttingBoard.fontStyle = FontStyles.Strikethrough;
    }
    public virtual void IngredientPulled()
    {
        if (_firstIngredientPulled) return;
        _firstIngredientPulled = true;

        _onIngredientPulled.Invoke();
    }

}
