using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class KitchenSubTutorial : MonoBehaviour
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
    [SerializeField] protected UnityEvent _onFryingDone;
    private bool _firstCook = false;
    [SerializeField] protected UnityEvent _onCookingStart;
    private bool _firstCookDone = false;
    [SerializeField] protected UnityEvent _onCookingDone;
    private bool _firstBaking = false;
    [SerializeField] protected UnityEvent _onBakingStart;
    private bool _firstSide = false;
    [SerializeField] protected UnityEvent _onSideBaked;
    private bool _firstBakingFlip = false;
    [SerializeField] protected UnityEvent _onBakingFlip;
    private bool _firstBakingDone = false;
    [SerializeField] protected UnityEvent _onBakingDone;
    private bool _firstIngredientCut = false;
    [SerializeField] protected UnityEvent _onIngredientCut;
    private bool _firstIngredientOnBoard = false;
    [SerializeField] protected UnityEvent _onIngredientToCuttingBoard;
    private bool _firstIngredientPulled = false;
    [SerializeField] protected UnityEvent _onIngredientPulled;
    private bool _firstIngredientAdded = false;
    [SerializeField] protected UnityEvent _onIngredientToDish;
    [SerializeField] protected UnityEvent _onFirstIngredientToDish;

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

    protected void StrikeThroughText(TextMeshProUGUI text)
    {
        if (text != null) text.fontStyle = FontStyles.Strikethrough;
    }

    public virtual void Execute() => _onExecute.Invoke();
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
        if (_firstBakingFlip) return;
        _firstBakingFlip = true;

        _onBakingFlip.Invoke();
        StrikeThroughText(_flipMeat);
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

    public virtual void IngredientAddedToDish()
    {
        _onIngredientToDish.Invoke();
        if (_firstIngredientAdded) return;
        _firstIngredientAdded = true;
        _onFirstIngredientToDish.Invoke();
    }

    public virtual void IngredientDone(IIngredient ingredient)
    {

    }
}
