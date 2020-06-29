using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : MonoBehaviour, IControllable, ISubject, IGameHandlerObserver
{
    public enum State
    {
        Withered = -1,

        Undifined = 0,
        Rough = 1,
        Dug,
        Planted,
        Growing,
        Decay,
        Healing,
        Grown,
        Harvested
    };

    public enum StateReady
    {
        OnCooldown = -1,
        InvalidAdvancement = -2,

        Ready = 1
    }

    [SerializeField]
    private ProgressBar _progressBar;
    private bool _neglectCooldown = true;
    private bool _paused = false;
    private bool _hasBeenPoisened = false;
    private float _cooldown = 3.0f;

    [SerializeField]
    private SFX soundEffectManager;

    [SerializeField]
    private GameObject _harvestPotatoPrefab;

    [Header("State meshes")]
    [SerializeField]
    private GameObject _dirtMound;
    public bool _interActable = true;

    [Header("Plant positions")]
    [SerializeField] private GameObject[] _plantPositions = new GameObject[4];

    [Header("State machine")]
    [SerializeField] private FarmPlotStateProvider _stateProvider;
    [SerializeField] private FarmPlotState _currentState;
    private Stack<FarmPlotState> _states = new Stack<FarmPlotState>();

    // Observers
    private List<IObserver> _observers = new List<IObserver>();

    private bool _updateHasBeenCalled = false;
    private bool _cultivateAfterCooldown = false;
    private bool _popStateAfterCooldown = false;
    private State _cultivateStateAfterCooldown;
    private bool _isOnCooldown = false;
    private float _cooldownTimer = 0.0f;

    private void Awake()
    {
        _progressBar?.SetActive(false);
        gameObject.tag = "FarmPlot";
    }

    private void Start()
    {
        var gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        _states.Push(_stateProvider.GetNullState());
        SetState(_currentState);
        _neglectCooldown = true;
    }

    public void SetInteractable(bool set)
    {
        _interActable = set;
    }

    public void SetStartState(State state)
    {
        return;
        if (_updateHasBeenCalled) return;
        ClearAllStates();
        SetState(state);
        _neglectCooldown = true;
    }

    public void ClearAllStates()
    {
        while(_states.Count > 1)
        {
            FarmPlotState state = _states.Pop();
            state.ExitState();
        }
    }

    private void Update()
    {
        if (_paused) return;
        _updateHasBeenCalled = true;
        if (_isOnCooldown) _cooldownTimer += Time.deltaTime;
        if (_cooldownTimer >= _cooldown && _isOnCooldown)
        {
            _isOnCooldown = false;
            if (_cultivateAfterCooldown)
            {
                SetState(_cultivateStateAfterCooldown);
            }
            else if (_popStateAfterCooldown)
            {
                PopState();
                _popStateAfterCooldown = false;
            }
        }

        _states.Peek().Update();

        if (_progressBar != null)
        {
            if (_cooldownTimer <= _cooldown && !_neglectCooldown && _isOnCooldown)
            {
                _progressBar.SetFillColor(new Color(102 / 255.0f, 77 / 255.0f, 63 / 255.0f));
                _progressBar.SetActive(true);
                _progressBar.SetPercentage(_cooldownTimer / _cooldown);
            }
            else if(_states.Peek().SetStateProgress(_progressBar))
            {
                _progressBar.SetActive(false);
            }
        }
    }

    #region Outside Actions (Dig/Plant/etc)

    public static bool Dig(FarmPlot plot, float cooldown, FarmTool tool)
    {
        return plot.Dig(cooldown, tool);
    }

    public static bool Plant(FarmPlot plot, float cooldown, FarmTool tool)
    {
        return plot.Plant(cooldown, tool);
    }

    public static bool Water(FarmPlot plot, float cooldown, FarmTool tool)
    {
        return plot.Water(cooldown, tool);
    }

    public static bool Heal(FarmPlot plot, float cooldown, FarmTool tool)
    {
        return plot.Heal(cooldown, tool);
    }

    public bool Dig(float cooldown, FarmTool tool)
    {
        var readyForState = ReadyForState(State.Dug);
        if (readyForState == StateReady.Ready && _interActable)
        {
            soundEffectManager.SoundDig();

            _cooldown = cooldown;
            CultivateAfterCooldown(State.Dug);
            return true;
        }
        else
        {
            if (readyForState == StateReady.OnCooldown) Notify(new PlotOnCooldownWarningEvent(this, tool));
            else Notify(new WrongToolOnPlotWarningEvent(this, tool));
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Plant(float cooldown, FarmTool tool)
    {
        var readyForState = ReadyForState(State.Planted);
        if (readyForState == StateReady.Ready && _interActable)
        {
            _cooldown = cooldown;
            CultivateAfterCooldown(State.Planted);
            return true;
        }
        else
        {
            if (readyForState == StateReady.OnCooldown) Notify(new PlotOnCooldownWarningEvent(this, tool));
            else Notify(new WrongToolOnPlotWarningEvent(this, tool));
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Water(float cooldown, FarmTool tool)
    {
        FarmPlot.StateReady readyForGrow = ReadyForState(State.Growing);
        FarmPlot.StateReady readyForHeal = ReadyForState(State.Healing);
        if (readyForGrow == StateReady.Ready && readyForHeal == StateReady.InvalidAdvancement && _interActable)
        {
            soundEffectManager.SoundWater();

            _cooldown = cooldown;
            CultivateAfterCooldown(State.Growing);
            return true;
        }
        else
        {
            if (readyForGrow == StateReady.OnCooldown || readyForHeal == StateReady.OnCooldown) Notify(new PlotOnCooldownWarningEvent(this, tool));
            else Notify(new WrongToolOnPlotWarningEvent(this, tool));
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Heal(float cooldown, FarmTool tool)
    {
        FarmPlot.StateReady readyForHeal = ReadyForState(State.Healing);
        if (readyForHeal == StateReady.Ready && _interActable)
        {
            soundEffectManager.SoundPesticide();

            _cooldown = cooldown;
            SetState(State.Healing);
            return true;
        }
        else
        {
            if (readyForHeal == StateReady.OnCooldown) Notify(new PlotOnCooldownWarningEvent(this, tool));
            else Notify(new WrongToolOnPlotWarningEvent(this, tool));
            Debug.Log("Not allowed");
            return false;
        }
    }

    #endregion

    private StateReady ReadyForState(State state)
    {
        if (_cooldownTimer >= _cooldown || _neglectCooldown)
        {
            return _states.Peek().ReadyForState(state);
        }
        else
        {
            return StateReady.OnCooldown;
        }
    }

    public void CultivateAfterCooldown(State state)
    {
        InformObserversOfStartStateSwitch(state, _states.Peek().GetState());
        _isOnCooldown = true;
        _cultivateAfterCooldown = true;
        _cultivateStateAfterCooldown = state;
        _cooldownTimer = 0.0f;
        _neglectCooldown = false;
    }

    public void PopStateAfterCooldown()
    {
        FarmPlotState now = _states.Pop();
        InformObserversOfStartStateSwitch(_states.Peek().GetState(), now.GetState());
        _states.Push(now);
        _isOnCooldown = true;
        _popStateAfterCooldown = true;
        _cooldownTimer = 0.0f;
        _neglectCooldown = false;
    }

    public void SetState(FarmPlot.State state)
    {
        SetState(_stateProvider.RequestStateObjectForState(state));
    }

    public void SetState(FarmPlotState state, bool popPrevious = false)
    {
        FarmPlotState newState = Instantiate(state);
        newState.EnterState(this);
        if (_states.Count > 0)
        {
            InformObserversOfStateSwitch(newState.GetState(), _states.Peek().GetState());
        }
        else InformObserversOfStateSwitch(newState.GetState(), State.Undifined);

        _states.Peek().UnLoad(_states.Peek());
        if (popPrevious) _states.Pop().ExitState();
        _states.Push(newState);
        _currentState = _states.Peek();

        _neglectCooldown = false;
        _cultivateAfterCooldown = false;
    }

    public void PopState(bool informLoad = true)
    {
        FarmPlotState state = _states.Pop();
        state.ExitState();
        _states.Peek().ReLoad(state);
    }

    public void ClearPlants()
    {
        for (var i = 0; i < _plantPositions.Length; ++i)
        {
            var loopCount = _plantPositions[i].transform.childCount;
            while (loopCount != 0)
            {
                Destroy(_plantPositions[i].transform.GetChild(0).gameObject);
                --loopCount;
            }
        }
    }

    public void SetPlants(List<GameObject> plantMeshes)
    {
        if (plantMeshes == null || plantMeshes.Count == 0) return;
        for (var i = 0; i < _plantPositions.Length; ++i)
        {
            var plant = Instantiate(plantMeshes[Random.Range(0, plantMeshes.Count)]);
            plant.transform.SetParent(_plantPositions[i].transform);
            plant.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void EnableDirtMounds(bool active)
    {
        _dirtMound.SetActive(active);
    }

    public bool HasBeenPoisened()
    {
        return _hasBeenPoisened;
    }

    public void Decay()
    {
        if (ReadyForState(State.Decay) == StateReady.Ready) SetState(State.Decay);
    }

    public bool Harvest()
    {
        InformObserversOfHarvest();
        if (ReadyForState(State.Harvested) == StateReady.Ready)
        {
            this.ClearAllStates();
            SetState(State.Rough);
            return true;
        }
        else
        {
            return false;
        }
    }

    #region IControllable

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        if (_states.Peek().GetState() == State.Grown)
        {
            var copy = Instantiate(_harvestPotatoPrefab);
            soundEffectManager.SoundUproot();
            return copy;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region ISubject

    public void Register(IObserver observer)
    {
        if (observer is IFarmPlotObserver)
        {
            _observers.Add(observer as IFarmPlotObserver);
        }
    }

    public void UnRegister(IObserver observer)
    {
        if (observer is IFarmPlotObserver)
        {
            _observers.Remove(observer as IFarmPlotObserver);
        }
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }

    #endregion

    #region Inform Observers

    private void InformObserversOfStartStateSwitch(State next, State current)
    {
        if (_observers == null) return;
        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is IFarmPlotObserver)
                (_observers[i] as IFarmPlotObserver).OnPlotStartStateSwitch(next, current, this);
        }
    }

    private void InformObserversOfStateSwitch(State current, State previous)
    {
        if (_observers == null) return;
        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is IFarmPlotObserver)
                (_observers[i] as IFarmPlotObserver).OnPlotStateSwitch(current, previous, this);
        }
    }

    private void InformObserversOfHarvest()
    {
        if (_observers == null) return;
        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is IFarmPlotObserver) (_observers[i] as IFarmPlotObserver).OnPlotHarvest(this);
        }
    }

    #endregion

    #region GamehandlerObserver

    public void OnPause()
    {
        if (!_paused) _paused = true;
    }

    public void OnContinue()
    {
        if (_paused) _paused = false;
    }

    public void OnFinish()
    {
        if (!_paused) _paused = true;
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
    }

    #endregion
}