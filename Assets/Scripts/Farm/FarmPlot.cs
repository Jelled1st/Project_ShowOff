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
        Decay,
        Grown,
    };

    [SerializeField] private float _cooldown = 3.0f;
    private State _state = State.Rough;
    private float _timeSinceLastCultivation = 0.0f;
    private bool _freeUseForStart = true;

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

    void Awake()
    {
        this.gameObject.name = "FarmPlot";
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
        if (ReadyForState(State.Grown))
        {
            CultivateToState(State.Grown);
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
            switch(_state)
            {
                case State.Withered:
                    if (state == State.Dug) return true;
                    else return false;
                case State.Rough:
                    if (state == State.Dug) return true;
                    else return false;
                case State.Dug:
                    if (state == State.Planted) return true;
                    else return false;
                case State.Planted:
                    if (state == State.Grown || state == State.Decay) return true;
                    else return false;
                case State.Decay:
                    if (state == State.Planted) return true;
                    else return false;
                case State.Grown:
                    return false;
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
                _dirtMound.SetActive(true);
                SetPlants(_plantWitheredMeshes);
                break;
            case State.Rough:
                _dirtMound.SetActive(false);
                break;
            case State.Dug:
                _dirtMound.SetActive(true);
                break;
            case State.Planted:
                _dirtMound.SetActive(true);
                SetPlants(_plantGrowingMeshes);
                break;
            case State.Decay:
                _dirtMound.SetActive(true);
                SetPlants(_plantDecayingMeshes);
                break;
            case State.Grown:
                _dirtMound.SetActive(true);
                SetPlants(_plantGrownMeshes);
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
        CultivateToState(State.Decay);
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
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<FarmPlot>());
        Destroy(copy.GetComponent<BoxCollider>());
        return copy;
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
