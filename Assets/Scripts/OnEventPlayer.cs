using System.Collections;
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
        public UnityEvent<FarmTool> onToolCooldownUseTool;
    }

    [System.Serializable]
    public struct DishEventFunctions
    {
        public UnityEvent onDishFinish;
        public List<DishIngredientAddUnityEvent> onDishAddIngredient;
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

    [SerializeField] private GameObject _gameHandler;
    [SerializeField] private TouchController _touchController;
    [SerializeField] private List<FarmPlot> _farmPlots;
    [SerializeField] private List<Dish> _dishes;

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
        SubscribeToDishes();
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
    }
    #endregion

    public void OnNotify(AObserverEvent observerEvent)
    {
        if (observerEvent is SwarmSpawnEvent)
        {
            this.Subscribe((observerEvent as SwarmSpawnEvent).swarm);
            _farmEvents.onSwarmSpawn.Invoke();
        }
        else if(observerEvent is ToolOnCooldownWarningEvent)
        {
            _farmEvents.onToolCooldownUse.Invoke();
            _farmEvents.onToolCooldownUseTool.Invoke((observerEvent as ToolOnCooldownWarningEvent).farmTool);
        }
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
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }
}
