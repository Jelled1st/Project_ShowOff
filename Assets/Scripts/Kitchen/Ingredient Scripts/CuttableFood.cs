using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class CuttableFood : MonoBehaviour, IControllable, IIngredient, ISubject
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private GameObject _currentState;
    [SerializeField] private List<GameObject> _cutStates;
    [HideInInspector] public CuttingBoard cuttingBoard = null;
    protected int _currentStateIndex = 0;

    private List<IObserver> _observers = new List<IObserver>();

    protected void Awake()
    {
        this.gameObject.tag = "Ingredient";
    }

    // Start is called before the first frame update
    protected void Start()
    {
        if (_cutStates != null && (_cutStates.Count == 0 || _cutStates[0] !=_currentState))
        {
            _cutStates.Insert(0, _currentState);
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public virtual bool Cut()
    {
        if (_currentStateIndex < _cutStates.Count - 1)
        {
            Transform current = _currentState.transform;
            Destroy(_currentState);
            _currentState = Instantiate(_cutStates[++_currentStateIndex], this.transform);
            _currentState.transform.localPosition = new Vector3(0, 0, 0);
            _currentState.transform.position = current.position;
            Notify(new CuttableCut(this, _currentStateIndex, ReadyForDish()));
            return true;
        }
        else return false;
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

        Destroy(this.gameObject);
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public GameObject GetDishMesh()
    {
        if(ReadyForDish())
        {
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<CuttableFood>());
            Destroy(copy.GetComponent<Collider>());
            Renderer[] renderers = copy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }
            return copy;
        }
        return _cutStates[_cutStates.Count-1];
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
        if(!(droppedOn is CuttingBoard))
        {
            cuttingBoard?.RequestRemoveSelected(this);
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public virtual GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<CuttableFood>());
        Collider[] colliders = copy.GetComponentsInChildren<Collider>();
        Rigidbody[] rbs = copy.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rbs.Length; ++i)
        {
            Destroy(rbs[i]);
        }
        for (int i = 0; i < colliders.Length; ++i)
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
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}
