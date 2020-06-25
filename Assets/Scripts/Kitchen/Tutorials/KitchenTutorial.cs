using System.Collections.Generic;
using UnityEngine;

public class KitchenTutorial : MonoBehaviour, IDishObserver
{
    [SerializeField]
    private KitchenSubTutorial _burgerTutorial;

    [SerializeField]
    private KitchenSubTutorial _ccFriesTutorial;

    [SerializeField]
    private KitchenSubTutorial _fishNChipsTutorial;

    [SerializeField]
    private FryFryer _fryer;

    [SerializeField]
    private FryingPan _pan;

    [SerializeField]
    private CookingPan _cooker;

    [SerializeField]
    private CuttingBoard _cuttingBoard;

    [HideInInspector]
    public KitchenGameHandler gameHandler;

    private KitchenSubTutorial _activeTutorial;

    private void Start()
    {
        _burgerTutorial?.DisableAllElements();
        _ccFriesTutorial?.DisableAllElements();
        _fishNChipsTutorial?.DisableAllElements();
        Subscribe(_fryer);
        Subscribe(_pan);
        Subscribe(_cooker);
        Subscribe(_cuttingBoard);
    }

    public void ChooseDish(Dish dish)
    {
        Subscribe(dish);
        var sideDishes = dish.GetSideDishesLeft();
        for (var i = 0; i < sideDishes.Count; ++i)
        {
            Subscribe(sideDishes[i]);
        }

        gameHandler.SubscribeToAllIngredients(this);
        if (dish.GetDishType() == Dish.DishTypes.BurgerAndFries)
        {
            _activeTutorial = _burgerTutorial;
        }
        else if (dish.GetDishType() == Dish.DishTypes.ChiliCheeseFries)
        {
            _activeTutorial = _ccFriesTutorial;
        }
        else if (dish.GetDishType() == Dish.DishTypes.FishAndChips)
        {
            _activeTutorial = _fishNChipsTutorial;
        }

        if (_activeTutorial != null) _activeTutorial.Execute();
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
        if (observerEvent is BakingStartEvent)
        {
            _activeTutorial?.BakingStart();
        }
        else if (observerEvent is SideBakedEvent)
        {
            _activeTutorial?.SideBakedDone();
        }
        else if (observerEvent is BakingDoneEvent)
        {
            _activeTutorial?.BakingDone();
        }
        else if (observerEvent is BakingFlipEvent)
        {
            _activeTutorial?.BakingFlip();
        }
        else if (observerEvent is CookingStartEvent)
        {
            _activeTutorial?.CookingStart();
        }
        else if (observerEvent is CookingDoneEvent)
        {
            _activeTutorial?.CookingDone();
        }
        else if (observerEvent is FryerStartEvent)
        {
            _activeTutorial?.FryingStart();
        }
        else if (observerEvent is FryerStopEvent)
        {
            _activeTutorial?.FryingDone();
        }
        else if (observerEvent is CuttableCutEvent)
        {
            _activeTutorial?.IngredientCut();
        }
        else if (observerEvent is CuttableCutUpEvent)
        {
            _activeTutorial?.IngredientCutUp();
        }
        else if (observerEvent is CuttableOnCuttingBoardEvent)
        {
            _activeTutorial?.IngredientToCuttingBoard();
        }
        else if (observerEvent is PullablePulledEvent)
        {
            _activeTutorial?.IngredientPulled();
        }
        else if (observerEvent is IngredientDoneEvent)
        {
            _activeTutorial?.IngredientDone((observerEvent as IngredientDoneEvent).ingredient);
        }
        else if (observerEvent is CookingStirEvent)
        {
            _activeTutorial?.CookingStir();
        }
        else if (observerEvent is DishStirEvent)
        {
            _activeTutorial?.DishStir();
        }
        else if (observerEvent is CookingAllIngredientsDoneEvent)
        {
            _activeTutorial?.CookingAllIngredientsDone();
        }
        else if (observerEvent is SideDishDraggedToMain)
        {
            _activeTutorial?.SideDishToMain();
        }
    }

    public void OnIngredientAdd(ISubject subject, IIngredient ingredient)
    {
        _activeTutorial?.IngredientAddedToDish(subject as Dish);
    }

    public void OnFinishDish(ISubject subject)
    {
        Debug.Log(subject + " is finished");
        _activeTutorial?.DishComplete(subject as Dish);
    }
}