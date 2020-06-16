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
    [SerializeField] UnityEvent _onFryingStart;
    [SerializeField] UnityEvent _onFryingDone;
    [SerializeField] UnityEvent _onCookingStart;
    [SerializeField] UnityEvent _onCookingDone;
    [SerializeField] UnityEvent _onBakingStart;
    [SerializeField] UnityEvent _onBakingFlip;
    [SerializeField] UnityEvent _onBakingDone;
    [SerializeField] UnityEvent _onIngredientCut;
    [SerializeField] UnityEvent _onIngredientToCuttingBoard;
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
        _onFryingStart.Invoke();
        if(_friesInFryer != null) _friesInFryer.fontStyle = FontStyles.Strikethrough;
    }
    public virtual void FryingDone() => _onFryingDone.Invoke();
    public virtual void CookingStart() => _onCookingStart.Invoke();
    public virtual void CookingDone() => _onCookingDone.Invoke();
    public virtual void BakingStart()
    {
        _onBakingStart.Invoke();
        if (_meatToFryingPan != null) _meatToFryingPan.fontStyle = FontStyles.Strikethrough;
    }

    public virtual void BakingDone() => _onBakingDone.Invoke();
    public virtual void IngredientCut()
    {
        _onIngredientCut.Invoke();
        if (_cutIngredient != null) _cutIngredient.fontStyle = FontStyles.Strikethrough;
    }
    public virtual void IngredientToCuttingBoard()
    {
        _onIngredientToCuttingBoard.Invoke();
        if (_dragIngredientToCuttingBoard != null) _dragIngredientToCuttingBoard.fontStyle = FontStyles.Strikethrough;
    }
    public virtual void IngredientPulled() => _onIngredientPulled.Invoke();
    public virtual void BakingFlip()
    {
        _onBakingFlip.Invoke();
        if (_flipMeat != null) _flipMeat.fontStyle = FontStyles.Strikethrough;
    }
}
