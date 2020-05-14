using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : MonoBehaviour, IControllable, ISubject
{
    public enum State
    {
        Withered = -1,

        Rough = 0,
        Dug,
        Planted,
        Growing,
        Decay,
        Grown,
        Harvested,
    };

    [SerializeField] private State _state = State.Rough;

    [SerializeField] private float _cooldown = 3.0f;
    [SerializeField] private float _timeTillGrown = 10.0f;
    [SerializeField] private float _timeTillWithered = 10.0f;
    private float _timeSinceLastCultivation = 0.0f;
    private bool _freeUseForStart = true;

    [SerializeField] private GameObject _harvestPotatoPrefab;

    [Header("State meshes")]
    [SerializeField] GameObject _dirtMound;
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

    private bool _debugLog = false;

    void Awake()
    {
        this.gameObject.tag = "FarmPlot";
    }

    // Start is called before the first frame update
    void Start()
    {
        CultivateToState(_state);
        _freeUseForStart = true;
    }

    public void SetStartState(State state)
    {
        if (_updateHasBeenCalled) return;
        CultivateToState(state);
        _freeUseForStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        _updateHasBeenCalled = true;
        _timeSinceLastCultivation += Time.deltaTime;
        if(ReadyForState(State.Grown))
        {
            CultivateToState(State.Grown);
        }
        else if(ReadyForState(State.Withered))
        {
            CultivateToState(State.Withered);
        }
    }

    public static bool Dig(FarmPlot plot)
    {
        return plot.Dig();
    }

    public static bool Plant(FarmPlot plot)
    {
        return plot.Plant();
    }

    public static bool Water(FarmPlot plot)
    {
        return plot.Water();
    }

    public bool Dig()
    {
        if (ReadyForState(State.Dug))
        {
            CultivateToState(State.Dug);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Plant()
    {
        if (ReadyForState(State.Planted))
        {
            CultivateToState(State.Planted);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Water()
    {
        if (ReadyForState(State.Growing))
        {
            CultivateToState(State.Growing);
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    private bool ReadyForState(State state)
    {
        if (_timeSinceLastCultivation >= _cooldown || _freeUseForStart)
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
                    if (_state == State.Rough || _state == State.Withered || _state == State.Harvested) return true;
                    else return false;
                case State.Planted:
                    if (_state == State.Dug) return true;
                    else return false;
                case State.Growing:
                    if (_state == State.Planted) return true;
                    else return false;
                case State.Decay:
                    if (_state == State.Growing) return true;
                    else return false;
                case State.Grown:
                    if (_state == State.Growing && _timeSinceLastCultivation >= _timeTillGrown) return true;
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

    private void CultivateToState(State state)
    {
        InformObserversOfStateSwitch(state, _state);
        _state = state;
        ClearPlants();
        switch (_state)
        {
            case State.Withered:
                if (_debugLog) Debug.Log("Withered!");
                _dirtMound.SetActive(false);
                SetPlants(_plantWitheredMeshes);
                break;
            case State.Rough:
                if (_debugLog) Debug.Log("Rough!");
                _dirtMound.SetActive(false);
                break;
            case State.Dug:
                if (_debugLog) Debug.Log("Dug!");
                _dirtMound.SetActive(true);
                break;
            case State.Planted:
                if (_debugLog) Debug.Log("Planted!");
                _dirtMound.SetActive(true);
                SetPlants(_plantGrowingMeshes);
                break;
            case State.Growing:
                if (_debugLog) Debug.Log("Growing!");
                _dirtMound.SetActive(true);
                SetPlants(_plantGrowingMeshes);
                break;
            case State.Decay:
                if (_debugLog) Debug.Log("Decay!");
                _dirtMound.SetActive(true);
                SetPlants(_plantDecayingMeshes);
                break;
            case State.Grown:
                if (_debugLog) Debug.Log("Grown!");
                _dirtMound.SetActive(true);
                SetPlants(_plantGrownMeshes);
                break;
            case State.Harvested:
                if (_debugLog) Debug.Log("Harvested!");
                _dirtMound.SetActive(false);
                break;
            default:
                break;
        }
        _timeSinceLastCultivation = 0.0f;
        _freeUseForStart = false;
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
            GameObject plant = Instantiate(plantMeshes[Random.Range(0, plantMeshes.Count)]);
            plant.transform.SetParent(_plantPositions[i].transform);
            plant.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void Decay()
    {
        if(ReadyForState(State.Decay)) CultivateToState(State.Decay);
    }

    public bool Harvest()
    {
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
    #endregion

    #region Inform Observers
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
}
