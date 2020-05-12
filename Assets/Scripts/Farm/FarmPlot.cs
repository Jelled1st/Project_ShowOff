using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : MonoBehaviour, IControllable
{
    enum State
    {
        Withered = -1,

        Rough = 0,
        Dug,
        Planted,
        Grown,
    };

    [SerializeField] private float _cooldown = 3.0f;
    private State _state = State.Rough;
    private float _timeSinceLastCultivation = 0.0f;
    private bool _freeUseForStart = true;

    [Header("State meshes")]
    [SerializeField] private Mesh _roughGround = null;
    [SerializeField] private Mesh _dugGround = null;
    [SerializeField] private Mesh _planted = null;
    [SerializeField] private Mesh _grown = null;
    [SerializeField] private Mesh _withered = null;
    private Dictionary<State, Mesh> _meshes;
    private bool useMeshSwitching = false;

    // Start is called before the first frame update
    void Start()
    {
        _meshes = new Dictionary<State, Mesh>();
        if (_roughGround != null) _meshes.Add(State.Rough, _roughGround);
        if (_dugGround != null) _meshes.Add(State.Dug, _roughGround);
        if (_planted != null) _meshes.Add(State.Planted, _roughGround);
        if (_grown != null) _meshes.Add(State.Grown, _roughGround);
        if (_withered != null) _meshes.Add(State.Withered, _roughGround);
        if (_meshes.Count != 5 && useMeshSwitching) Debug.LogError("Not all meshes have been assigned");
    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("Digging");
            Cultivate();
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
            Debug.Log("Planting");
            Cultivate();
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
            Debug.Log("Watering");
            Cultivate();
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
        if (_state + 1 == state && (_timeSinceLastCultivation >= _cooldown || _freeUseForStart))
        {
            return true;
        }
        else return false;
    }

    private void Cultivate()
    {
        ++_state;
        _timeSinceLastCultivation = 0.0f;
        _freeUseForStart = false;
        if(useMeshSwitching) GetComponent<MeshFilter>().sharedMesh = Instantiate(_meshes[_state]);
    }


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
}
