using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class KitchenTutorial : MonoBehaviour, IObserver
{
    [SerializeField] KitchenSubTutorial _burgerTutorial;
    [SerializeField] KitchenSubTutorial _ccFriesTutorial;
    [SerializeField] KitchenSubTutorial _fishNChipsTutorial;
    [SerializeField] FryFryer _fryer;
    [SerializeField] FryingPan _pan;
    [SerializeField] CookingPan _cooker;
    [SerializeField] CuttingBoard _cuttingBoard;
    [HideInInspector] public KitchenGameHandler gameHandler;
    private KitchenSubTutorial _activeTutorial;

    // Start is called before the first frame update
    void Start()
    {
        _burgerTutorial.DisableAllElements();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseDish(Dish dish)
    {
        Subscribe(dish);
        gameHandler.SubscribeToAllIngredients(this);
        if (dish.GetDishType() == Dish.DishTypes.BurgerAndFries)
        {
            _burgerTutorial.Execute();
            _activeTutorial = _burgerTutorial;
        }
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
        if(observerEvent is BakingStartEvent)
        {
            _activeTutorial.BakingStart();
        }
        else if(observerEvent is BakingDoneEvent)
        {
            _activeTutorial.BakingDone();
        }
        else if(observerEvent is BakingFlipEvent)
        {
            _activeTutorial.BakingFlip();
        }
        else if (observerEvent is CookingStartEvent)
        {
            _activeTutorial.CookingDone();
        }
        else if (observerEvent is CookingDoneEvent)
        {
            _activeTutorial.CookingStart();
        }
        else if (observerEvent is FryerStartEvent)
        {
            _activeTutorial.FryingStart();
        }
        else if (observerEvent is FryerStopEvent)
        {
            _activeTutorial.FryingDone();
        }
        else if (observerEvent is CuttableCutEvent)
        {
            _activeTutorial.IngredientCut();
        }
        else if(observerEvent is CuttableOnCuttingBoardEvent)
        {
            _activeTutorial.IngredientToCuttingBoard();
        }
        else if (observerEvent is PullablePulledEvent)
        {
            _activeTutorial.IngredientPulled();
        }
        else if(observerEvent is IngredientDoneEvent)
        {
            _activeTutorial.IngredientDone((observerEvent as IngredientDoneEvent).ingredient);
        }
    }
}
