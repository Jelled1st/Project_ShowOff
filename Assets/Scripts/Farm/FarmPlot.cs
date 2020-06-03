using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Harvested,
    };

    [SerializeField] private State _state = State.Rough;

    [SerializeField] private float _timeTillGrown = 10.0f;
    [Tooltip("Higher slowness means growing takes longer")] [SerializeField] private float _decayGrowSlowness = 2.0f;
    [SerializeField] private float _timeTillWithered = 10.0f;
    [SerializeField] private ProgressBar _progressBar;
    private float _timeSinceLastCultivation = 0.0f;
    private float _growTime;
    private bool _neglectCooldown = true;
    private static bool _paused = false;
    private bool _hasBeenPoisened = false;
    private float _cooldown = 3.0f;

    [SerializeField] private SFX soundEffectManager;

    [SerializeField] private GameObject _harvestPotatoPrefab;

    [Header("State meshes")]
    [SerializeField] GameObject _dirtMound;
    [SerializeField] List<GameObject> _plantPlantedMeshes;
    [SerializeField] List<GameObject> _plantGrowingMeshes;
    [SerializeField] List<GameObject> _plantDecayingMeshes;
    [SerializeField] List<GameObject> _plantWitheredMeshes;
    [SerializeField] List<GameObject> _plantGrownMeshes;
    private bool useMeshSwitching = false;

    [Header("Plant positions")]
    [SerializeField] GameObject[] _plantPositions = new GameObject[4];

    // Observers
    List<IFarmPlotObserver> _observers;

    private bool _updateHasBeenCalled = false;
    private bool _cultivateAfterCooldown = false;
    private State _cultivateStateAfterCooldown;
    private bool _isOnCooldown = false;
    private float _cooldownTimer = 0.0f;

    private const bool _debugLog = false;

    void Awake()
    {
        _progressBar?.SetActive(false);
        this.gameObject.tag = "FarmPlot";
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        CultivateToState(_state);
        _neglectCooldown = true;
    }

    public void SetStartState(State state)
    {
        if (_updateHasBeenCalled) return;
        CultivateToState(state);
        _neglectCooldown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_paused) return;
        _updateHasBeenCalled = true;
        _timeSinceLastCultivation += Time.deltaTime;
        if (_isOnCooldown) _cooldownTimer += Time.deltaTime;
        if(_cooldownTimer >= _cooldown && _isOnCooldown)
        {
            _isOnCooldown = false;
            if (_cultivateAfterCooldown)
            {
                CultivateToState(_cultivateStateAfterCooldown);
            }
        }
        if(_state == State.Growing)
        {
            _growTime += Time.deltaTime;
            //Debug.Log("Grow time: " + _growTime + " ( " + (_growTime >= _timeTillGrown) + " )");
        }
        else if(_state == State.Decay)
        {
            _growTime += Time.deltaTime / _decayGrowSlowness;
        }
        if (ReadyForState(State.Grown))
        {
            CultivateToState(State.Grown);
        }
        else if(ReadyForState(State.Withered))
        {
            CultivateToState(State.Withered);
        }
        if(_progressBar != null)
        {
            if (_state == State.Growing)
            {
                _progressBar.SetFillColor(new Color(30/255.0f, 30/255.0f, 200/255.0f));
                _progressBar.SetActive(true);
                float percentage = _growTime / _timeTillGrown;
                if (percentage <= 1.0f) _progressBar.SetPercentage(percentage);
                else _progressBar.SetPercentage(1.0f);
            }
            else if (_state == State.Decay)
            {
                _progressBar.SetFillColor(new Color(1f, 60/255.0f, 0));
                _progressBar.SetActive(true);
                _progressBar.SetPercentage(1 - _timeSinceLastCultivation / _timeTillWithered);
            }
            else if (_cooldownTimer <= _cooldown && !_neglectCooldown && _isOnCooldown)
            {
                _progressBar.SetFillColor(new Color(30/255.0f, 30/255.0f, 200/255.0f));
                _progressBar.SetActive(true);
                _progressBar.SetPercentage(_cooldownTimer / _cooldown);
            }
            else _progressBar.SetActive(false);
        }
    }

    #region Outside Actions (Dig/Plant/etc)
    public static bool Dig(FarmPlot plot, float cooldown)
    {
        return plot.Dig(cooldown);
    }

    public static bool Plant(FarmPlot plot, float cooldown)
    {
        return plot.Plant(cooldown);
    }

    public static bool Water(FarmPlot plot, float cooldown)
    {
        return plot.Water(cooldown);
    }

    public static bool Heal(FarmPlot plot, float cooldown)
    {
        return plot.Heal(cooldown);
    }

    public bool Dig(float cooldown)
    {
        if (ReadyForState(State.Dug))
        {
            soundEffectManager.SoundDig();

            _cooldown = cooldown;
            CultivateAfterCooldown(State.Dug);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Plant(float cooldown)
    {
        if (ReadyForState(State.Planted))
        {
            _cooldown = cooldown;
            CultivateAfterCooldown(State.Planted);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Water(float cooldown)
    {
        if (ReadyForState(State.Growing) && _state == State.Planted)
        {   
            soundEffectManager.SoundWater();

            _cooldown = cooldown;
            CultivateAfterCooldown(State.Growing);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Heal(float cooldown)
    {
        if(ReadyForState(State.Growing) && _state == State.Decay)
        {
            soundEffectManager.SoundPesticide();

            _cooldown = cooldown;
            CultivateToState(State.Healing);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }
    #endregion

    private bool ReadyForState(State state)
    {
        if (_cooldownTimer >= _cooldown || _neglectCooldown)
        {
            switch(state)
            {
                case State.Withered:
                    if (_state == State.Decay && _timeSinceLastCultivation >= _timeTillWithered) return true;
                    else return false;
                case State.Rough:
                    if (_state == State.Grown) return true;
                    return false;
                case State.Dug:
                    if (_state == State.Rough || _state == State.Withered || _state == State.Harvested || _state == State.Undifined) return true;
                    else return false;
                case State.Planted:
                    if (_state == State.Dug) return true;
                    else return false;
                case State.Growing:
                    if (_state == State.Planted || _state == State.Decay) return true;
                    else return false;
                case State.Decay:
                    if (_state == State.Growing) return true;
                    else return false;
                case State.Healing:
                    if (_state == State.Decay) return true;
                    else return false;
                case State.Grown:
                    if (_state == State.Growing && _growTime >= _timeTillGrown) return true;
                    else return false;
                case State.Harvested:
                    if (_state == State.Grown) return true;
                    else return false;
                default:
                    return false;
            }
        }
        else return false;
    }

    private void CultivateAfterCooldown(State state)
    {
        InformObserversOfStartStateSwitch(state, _state);
        _isOnCooldown = true;
        _cultivateAfterCooldown = true;
        _cultivateStateAfterCooldown = state;
        _cooldownTimer = 0.0f;
        _neglectCooldown = false;
    }

    private void CultivateToState(State state)
    {
        InformObserversOfStateSwitch(state, _state);
        State previousState = _state;
        _state = state;
        ClearPlants();
        _neglectCooldown = false;
        _cultivateAfterCooldown = false;
        switch (_state)
        {
            case State.Withered:
                if (_debugLog) Debug.Log("Withered!");
                _growTime = 0.0f;
                _dirtMound.SetActive(false);
                SetPlants(_plantWitheredMeshes);
                break;
            case State.Rough:
                if (_debugLog) Debug.Log("Rough!");
                _growTime = 0.0f;
                _dirtMound.SetActive(false);
                break;
            case State.Dug:
                if (_debugLog) Debug.Log("Dug!");
                _growTime = 0.0f;
                _dirtMound.SetActive(true);
                break;
            case State.Planted:
                if (_debugLog) Debug.Log("Planted!");
                _growTime = 0.0f;
                _dirtMound.SetActive(true);
                SetPlants(_plantPlantedMeshes);
                break;
            case State.Growing:
                if (_debugLog) Debug.Log("Growing!");
                if (previousState == State.Decay) _growTime = Mathf.Min(_growTime, _timeTillGrown - 1.0f);
                _dirtMound.SetActive(true);
                SetPlants(_plantGrowingMeshes);
                _neglectCooldown = true;
                break;
            case State.Decay:
                if (_debugLog) Debug.Log("Decay!");
                _hasBeenPoisened = true;
                _dirtMound.SetActive(true);
                SetPlants(_plantDecayingMeshes);
                _neglectCooldown = true;
                break;
            case State.Healing:
                if (_debugLog) Debug.Log("Healing!");
                _dirtMound.SetActive(true);
                SetPlants(_plantDecayingMeshes);
                CultivateAfterCooldown(State.Growing);
                break;
            case State.Grown:
                if (_debugLog) Debug.Log("Grown!");
                _growTime = 0.0f;

                soundEffectManager.SoundPlantGrowth();

                _dirtMound.SetActive(true);
                SetPlants(_plantGrownMeshes);
                _neglectCooldown = true;
                break;
            case State.Harvested:
                if (_debugLog) Debug.Log("Harvested!");
                _growTime = 0.0f;
                _dirtMound.SetActive(false);
                break;
            default:
                break;
        }
        _timeSinceLastCultivation = 0;
    }

    private void ClearPlants()
    {
        for(int i = 0; i < _plantPositions.Length; ++i)
        {
            if(_plantPositions[i].transform.childCount > 0) Destroy(_plantPositions[i].transform.GetChild(0).gameObject);
        }
    }

    private void SetPlants(List<GameObject> plantMeshes)
    {
        if (plantMeshes == null || plantMeshes.Count == 0) return;
        for(int i = 0; i < _plantPositions.Length; ++i)
        {
            GameObject plant = Instantiate(plantMeshes[UnityEngine.Random.Range(0, plantMeshes.Count)]);
            plant.transform.SetParent(_plantPositions[i].transform);
            plant.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public bool HasBeenPoisened()
    {
        return _hasBeenPoisened;
    }

    public void Decay()
    {
        if(ReadyForState(State.Decay)) CultivateToState(State.Decay);
    }

    public bool Harvest()
    {
        InformObserversOfHarvest();
        if (ReadyForState(State.Harvested))
        {
            CultivateToState(State.Harvested);
            return true;
        }
        else return false;
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
        if (_state == State.Grown)
        {
            GameObject copy = Instantiate(_harvestPotatoPrefab);
            soundEffectManager.SoundUproot();
            return copy;
        }
        else return null;
    }
    #endregion

    #region ISubject
    public void Register(IObserver observer)
    {
        if (_observers == null) _observers = new List<IFarmPlotObserver>();
        if(observer is IFarmPlotObserver)
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

    }
    #endregion

    #region Inform Observers
    private void InformObserversOfStartStateSwitch(State next, State current)
    {
        if (_observers == null) return;
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPlotStartStateSwitch(next, current, this);
        }
    }

    private void InformObserversOfStateSwitch(State current, State previous)
    {
        if (_observers == null) return;
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPlotStateSwitch(current, previous, this);
        }
    }

    private void InformObserversOfHarvest()
    {
        if (_observers == null) return;
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPlotHarvest(this);
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
