using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEventPlayer : MonoBehaviour, IGameHandlerObserver, IFarmPlotObserver, IControlsObserver, ISwarmObserver, IDishObserver
{
    #region Event functions
    [System.Serializable]
    public struct FarmEventsFunctions
    {
        [Tooltip("Plot changed")]
        public List<PlotChangeUnityEvent> onPlotChange;
        [Tooltip("Cooldown has to play before change, newState is the state that it will be after the cooldown")]
        public List<PlotChangeUnityEvent> onPlotChangeStart;
        public UnityEvent onPlotHarvest;

        public UnityEvent onSwarmSpawn;
        public UnityEvent onSwarmDestroy;
        public UnityEvent onBugSpawn;
        public UnityEvent onFirstBugSpawn;
        public UnityEvent onBugSpawnFail;
        public UnityEvent onBugKill;
        public UnityEvent onSwarmFlee;

        public UnityEvent onToolCooldownUse;
        public UnityEvent onPlotCooldownUse;
        public UnityEvent onWrongToolOnPlot;
        public UnityEvent onFarmPlotInactive;
    }

    [System.Serializable]
    public struct DishEventFunctions
    {
        public UnityEvent onDishFinish;
        public List<DishIngredientAddUnityEvent> onDishAddIngredient;
        public UnityEvent onBakingStart;
        public UnityEvent onBakingDone;
        public UnityEvent onBakingBurnt;
        public UnityEvent onBakingStop;

        public UnityEvent onCookingStart;
        public UnityEvent onCookingDone;
        public UnityEvent onCookingStop;
        public UnityEvent onCookingStir;

        public UnityEvent onFryerStart;
        public UnityEvent onFryerStop;

        public UnityEvent onCutCuttableHard;
        public UnityEvent onCutCuttableSoft;

        public UnityEvent onDishStir;
    }

    [System.Serializable]
    public struct ControlsEventFucntions
    {
        public UnityEvent onClick;
        public UnityEvent onPress;
        public UnityEvent onHold;
        public UnityEvent onHoldRelease;
        public UnityEvent onSwipe;
        public UnityEvent onDrag;
        public UnityEvent onDragDrop;
        public UnityEvent onDragDropFailed;
    }

    [System.Serializable]
    public struct GameEventFunctions
    {
        public UnityEvent onPlay;
        public UnityEvent onPause;
        public UnityEvent onFinish;
    }
    #endregion

    #region combining structs
    [System.Serializable]
    public struct CookingSubjects
    {
        public FryingPan fryingPan;
        public FryFryer fryFryer;
        public CookingPan cookingPan;
    }
    #endregion

    [SerializeField] private GameObject _gameHandler;
    [SerializeField] private TouchController _touchController;
    [SerializeField] private List<ISubject> _subjects;
    [SerializeField] private List<FarmPlot> _farmPlots;
    [SerializeField] private List<FarmTool> _farmTools;
    [SerializeField] private List<Dish> _dishes;
    [SerializeField] private CookingSubjects _cookingSubjects;

    [SerializeField] private GameEventFunctions _gameEvents;
    [SerializeField] private ControlsEventFucntions _controllerEvents;
    [SerializeField] private FarmEventsFunctions _farmEvents;
    [SerializeField] private DishEventFunctions _dishEvents;

    private bool _bugHasSpawned = false;

    public void Start()
    {
        SubscribeToGameHandler();
        SubscribeToTouchController();
        SubscribeToFarmPlots();
        SubscribeToFarmTools();
        SubscribeToDishes();
        SubscribeToOther();
        Swarm.RegisterStatic(this);
    }

    #region Subscribing
    private void SubscribeToGameHandler()
    {
        if (_gameHandler == null)
        {
            _gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        }
        else Subscribe(_gameHandler.GetComponent<ISubject>());
    }
    private void SubscribeToTouchController()
    {
        if (_touchController == null)
        {
            GameObject controllerGO = GameObject.FindGameObjectWithTag("Controller");
            _touchController = controllerGO.GetComponent<TouchController>();
        }
        Subscribe(_touchController);
    }
    private void SubscribeToFarmPlots()
    {
        if (_farmPlots.Count == 0)
        {
            GameObject[] farmPlotsGOs = GameObject.FindGameObjectsWithTag("FarmPlot");
            for (int i = 0; i < farmPlotsGOs.Length; ++i)
            {
                FarmPlot farmPlot = farmPlotsGOs[i].GetComponent<FarmPlot>();
                _farmPlots.Add(farmPlot);
                Subscribe(farmPlot);
            }
        }
        else
        {
            for (int i = 0; i < _farmPlots.Count; ++i)
            {
                Subscribe(_farmPlots[i]);
            }
        }
    }
    private void SubscribeToFarmTools()
    {
        if (_farmTools.Count == 0)
        {
            GameObject[] farmToolsGOs = GameObject.FindGameObjectsWithTag("FarmTool");
            for (int i = 0; i < farmToolsGOs.Length; ++i)
            {
                FarmTool farmTool = farmToolsGOs[i].GetComponent<FarmTool>();
                _farmTools.Add(farmTool);
                Subscribe(farmTool);
            }
        }
        else
        {
            for (int i = 0; i < _farmTools.Count; ++i)
            {
                Subscribe(_farmTools[i]);
            }
        }
    }
    private void SubscribeToDishes()
    {
        if (_dishes.Count == 0)
        {
            GameObject[] dishesGO = GameObject.FindGameObjectsWithTag("Dish");
            for (int i = 0; i < dishesGO.Length; ++i)
            {
                Dish dish = dishesGO[i].GetComponent<Dish>();
                _dishes.Add(dish);
                Subscribe(dish);
            }
        }
        else
        {
            for (int i = 0; i < _dishes.Count; ++i)
            {
                Subscribe(_dishes[i]);
            }
        }

        if(_cookingSubjects.cookingPan != null )Subscribe(_cookingSubjects.cookingPan);
        if(_cookingSubjects.fryFryer != null) Subscribe(_cookingSubjects.fryFryer);
        if(_cookingSubjects.fryingPan != null) Subscribe(_cookingSubjects.fryingPan);
    }

    private void SubscribeToOther()
    {
        if (_subjects == null || _subjects.Count == 0) return;
        for(int i = 0; i < _subjects.Count; ++i)
        {
            Subscribe(_subjects[i]);
        }
    }
    #endregion

    public void OnNotify(AObserverEvent observerEvent)
    {
        if (InvokeFarmEvents(observerEvent)) ;
        else if (InvokeDishEvents(observerEvent)) ;
    }

    private bool InvokeFarmEvents(AObserverEvent observerEvent)
    {
        if (observerEvent is SwarmSpawnEvent)
        {
            this.Subscribe((observerEvent as SwarmSpawnEvent).swarm);
            _farmEvents.onSwarmSpawn.Invoke();
            return true;
        }
        else if (observerEvent is ToolOnCooldownWarningEvent)
        {
            _farmEvents.onToolCooldownUse.Invoke();
            return true;
        }
        else if (observerEvent is PlotOnCooldownWarningEvent)
        {
            _farmEvents.onPlotCooldownUse.Invoke();
            return true;
        }
        else if (observerEvent is WrongToolOnPlotWarningEvent)
        {
            _farmEvents.onWrongToolOnPlot.Invoke();
            return true;
        }
        else if (observerEvent is FarmPlotIsInactiveWarningEvent)
        {
            _farmEvents.onFarmPlotInactive.Invoke();
            return true;
        }
        return false;
    }

    private bool InvokeDishEvents(AObserverEvent observerEvent)
    {
        if (observerEvent is BakingStartEvent)
        {
            _dishEvents.onBakingStart.Invoke();
            return true;
        }
        else if (observerEvent is BakingDoneEvent)
        {
            _dishEvents.onBakingDone.Invoke();
            return true;
        }
        else if (observerEvent is BakingStartBurnEvent)
        {
            _dishEvents.onBakingBurnt.Invoke();
            return true;
        }
        else if (observerEvent is BakingStopEvent)
        {
            _dishEvents.onBakingStop.Invoke();
            return true;
        }
        else if (observerEvent is CookingStartEvent)
        {
            _dishEvents.onCookingStart.Invoke();
            return true;
        }
        else if (observerEvent is CookingDoneEvent)
        {
            _dishEvents.onCookingDone.Invoke();
            return true;
        }
        else if (observerEvent is CookingStopEvent)
        {
            _dishEvents.onCookingStop.Invoke();
            return true;
        }
        else if (observerEvent is CookingStirEvent)
        {
            _dishEvents.onCookingStir.Invoke();
            return true;
        }
        else if (observerEvent is CuttableCutEvent)
        {
            CuttableCutEvent cuttableCut = observerEvent as CuttableCutEvent;
            if (cuttableCut.isHard)
            {
                _dishEvents.onCutCuttableHard.Invoke();
                return true;
            }
            else
            {
                _dishEvents.onCutCuttableSoft.Invoke();
                return true;
            }
        }
        else if (observerEvent is FryerStartEvent)
        {
            _dishEvents.onFryerStart.Invoke();
            return true;
        }
        else if (observerEvent is FryerStopEvent)
        {
            _dishEvents.onFryerStop.Invoke();
            return true;
        }
        else if(observerEvent is DishStirEvent)
        {
            _dishEvents.onDishStir.Invoke();
            return true;
        }
        return false;
    }

    #region ISwarmObserver
    public void OnBugKill(SwarmUnit unit)
    {
        _farmEvents.onBugKill.Invoke();
    }

    public void OnBugSpawn(SwarmUnit unit)
    {
        if(!_bugHasSpawned)
        {
            _bugHasSpawned = true;
            _farmEvents.onFirstBugSpawn.Invoke();
        }
        _farmEvents.onBugSpawn.Invoke();
    }

    public void OnBugspawnFail()
    {
        _farmEvents.onBugSpawnFail.Invoke();
    }

    public void OnFlee()
    {
        _farmEvents.onSwarmFlee.Invoke();
    }

    public void OnSwarmDestroy()
    {
        _farmEvents.onSwarmDestroy.Invoke();
    }
    #endregion

    #region IControlsObserver
    public void OnClick(ControllerHitInfo hitInfo)
    {
        _controllerEvents.onClick.Invoke();
    }

    public void OnDrag(Vector3 position, IControllable dragged, ControllerHitInfo hitInfo)
    {
        _controllerEvents.onDrag.Invoke();
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        _controllerEvents.onDragDrop.Invoke();
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
    {
        _controllerEvents.onDragDropFailed.Invoke();
    }

    public void OnHold(float holdTime, ControllerHitInfo hitInfo)
    {
        _controllerEvents.onHold.Invoke();
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
        _controllerEvents.onHoldRelease.Invoke();
    }

    public void OnPress(ControllerHitInfo hitInfo)
    {
        _controllerEvents.onPress.Invoke();
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo)
    {
        _controllerEvents.onSwipe.Invoke();
    }
    #endregion

    #region IGameHandlerObserver
    public void OnContinue()
    {
        _gameEvents.onPlay.Invoke();
    }

    public void OnFinish()
    {
        _gameEvents.onFinish.Invoke();
    }

    public void OnPause()
    {
        _gameEvents.onPause.Invoke();
    }
    #endregion

    #region IFarmPlotObserver
    public void OnPlotHarvest(FarmPlot plot)
    {
        _farmEvents.onPlotHarvest.Invoke();
    }

    public void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot)
    {
        for (int i = 0; i < _farmEvents.onPlotChangeStart.Count; ++i)
        {
            _farmEvents.onPlotChangeStart[i].TryInvoke(switchState, currentState);
        }
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        for (int i = 0; i < _farmEvents.onPlotChange.Count; ++i)
        {
            _farmEvents.onPlotChange[i].TryInvoke(state, previousState);
        }
    }
    #endregion

    #region IDishObserver
    public void OnFinishDish(ISubject subject)
    {
        _dishEvents.onDishFinish.Invoke();
    }

    public void OnIngredientAdd(ISubject subject, IIngredient ingredient)
    {
        for (int i = 0; i < _dishEvents.onDishAddIngredient.Count; ++i)
        {
            _dishEvents.onDishAddIngredient[i].TryInvoke(ingredient.GetIngredientType());
        }
    }
    #endregion

    public void Subscribe(ISubject subject)
    {
        if (subject != null) subject.Register(this);
        else Debug.LogError("OnEventPlayer: subject is null");
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }
}
