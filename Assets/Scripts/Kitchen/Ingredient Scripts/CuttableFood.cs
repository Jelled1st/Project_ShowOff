using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CuttableFood : MonoBehaviour, IControllable, IIngredient, ISubject
{
    [SerializeField]
    private IngredientType _ingredientType;

    [SerializeField]
    private float _ingredientHeight;

    [SerializeField]
    protected GameObject _currentState;

    [SerializeField]
    private List<GameObject> _cutStates;

    [HideInInspector]
    public CuttingBoard cuttingBoard = null;

    [SerializeField]
    private bool _isHard = false;

    protected int _currentStateIndex = 0;

    private List<IObserver> _observers = new List<IObserver>();

    protected void Awake()
    {
        gameObject.tag = "Ingredient";
    }

    protected void Start()
    {
        if (_cutStates != null && (_cutStates.Count == 0 || _cutStates[0] != _currentState))
        {
            _cutStates.Insert(0, _currentState);
        }
    }

    public virtual bool Cut()
    {
        if (_currentStateIndex < _cutStates.Count - 1)
        {
            var previous = _currentState.transform;
            Destroy(_currentState);
            _currentState = Instantiate(_cutStates[++_currentStateIndex], transform);
            _currentState.transform.localPosition = new Vector3(0, 0, 0);
            _currentState.transform.position = previous.position;
            _currentState.transform.rotation = previous.rotation;
            Notify(new CuttableCutEvent(this, _isHard, _currentStateIndex, ReadyForDish()));
            if (_currentStateIndex >= _cutStates.Count - 1)
            {
                Notify(new CuttableCutUpEvent(this, _isHard));
                Notify(new IngredientDoneEvent(this));
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    #region IIngredient

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return _currentStateIndex >= _cutStates.Count - 1;
    }

    public void AddedToDish()
    {
        cuttingBoard?.RequestRemoveSelected(this);
        Destroy(gameObject);
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public virtual GameObject GetDishMesh()
    {
        if (ReadyForDish())
        {
            var copy = Instantiate(gameObject);
            Destroy(copy.GetComponent<CuttableFood>());
            var renderers = copy.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }

            var colliders = copy.GetComponentsInChildren<Collider>();
            var rbs = copy.GetComponentsInChildren<Rigidbody>();
            for (var i = 0; i < rbs.Length; ++i)
            {
                Destroy(rbs[i]);
            }

            for (var i = 0; i < colliders.Length; ++i)
            {
                Destroy(colliders[i]);
            }

            return copy;
        }

        return _cutStates[_cutStates.Count - 1];
    }

    #endregion

    #region IControllable

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
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

    public virtual GameObject GetDragCopy()
    {
        var copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<CuttableFood>());
        var colliders = copy.GetComponentsInChildren<Collider>();
        var rbs = copy.GetComponentsInChildren<Rigidbody>();
        for (var i = 0; i < rbs.Length; ++i)
        {
            Destroy(rbs[i]);
        }

        for (var i = 0; i < colliders.Length; ++i)
        {
            Destroy(colliders[i]);
        }

        copy.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return copy;
    }

    #endregion

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}