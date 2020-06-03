using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEventPlayer : MonoBehaviour, IGameHandlerObserver, IFarmPlotObserver, IControlsObserver, ISwarmObserver, IDishObserver
{
    [System.Serializable]
    public struct FarmEventsFunctions
    {
        [Tooltip("Plot changed")]
        public PlotChangeUnityEvent onPlotChange;
        [Tooltip("Cooldown has to play before change, newState is the state that it will be after the cooldown")]
        public PlotChangeUnityEvent onPlotChangeStart;
        public UnityEvent onSwarmSpawn;
    }
    [System.Serializable]
    public struct DishEventFunctions
    {
        public UnityEvent onDishFinish;
        public DishIngredientAddUnityEvent onDishAddIngredient;
    }

    [SerializeField] private FarmEventsFunctions _farmEvents;
    [SerializeField] private DishEventFunctions _dishEvents;

    public void Start()
    {
        Swarm.RegisterStatic(this);
    }

    #region ISwarmObserver
    public void OnBugKill(SwarmUnit unit)
    {
    }

    public void OnBugSpawn(SwarmUnit unit)
    {
    }

    public void OnBugspawnFail()
    {
    }

    public void OnFlee()
    {
    }

    public void OnSwarmDestroy()
    {
    }
    #endregion

    #region IControlsObserver
    public void OnClick(ControllerHitInfo hitInfo)
    {
    }

    public void OnDrag(Vector3 position, IControllable dragged, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
    {
    }

    public void OnHold(float holdTime, ControllerHitInfo hitInfo)
    {
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
    }

    public void OnPress(ControllerHitInfo hitInfo)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo)
    {
    }
    #endregion

    #region IGameHandlerObserver
    public void OnContinue()
    {
    }

    public void OnFinish()
    {
    }

    public void OnPause()
    {
    }
    #endregion

    #region IFarmPlotObserver
    public void OnPlotHarvest(FarmPlot plot)
    {
    }

    public void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot)
    {
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
    }
    #endregion

    #region IDishObserver
    public void OnFinishDish(ISubject subject)
    {
        _dishEvents.onDishFinish.Invoke();
    }

    public void OnIngredientAdd(ISubject subject, IIngredient ingredient)
    {
        _dishEvents.onDishAddIngredient.TryInvoke(ingredient.GetIngredientType());
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

    public void OnNotify(AObserverEvent observerEvent)
    {
        if (observerEvent is SwarmSpawnEvent)
        {
            this.Subscribe((observerEvent as SwarmSpawnEvent).swarm);
            _farmEvents.onSwarmSpawn.Invoke();
        }
    }
}
