using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullableFood : MonoBehaviour, IIngredient, IControllable, ISubject
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private List<GameObject> _pullables;
    [SerializeField] private InvisibleOnDrag _invisibleOnDrag;
    private GameObject _currentDragPullable;

    private List<IObserver> _observers = new List<IObserver>();

    void Awake()
    {
        this.tag = "Ingredient";
    }

    void Start()
    {
        if(_invisibleOnDrag != null) _invisibleOnDrag.active = false;
    }

    #region IIngredient
    public void AddedToDish()
    {
        Destroy(this);
    }

    public GameObject GetDishMesh()
    {
        return null;
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return _pullables.Count == 0;
    }
    #endregion

    private void DropCurrentPullable()
    {
        if (ReadyForDish()) return;
    }

    #region IControllable

    public GameObject GetDragCopy()
    {
        if(ReadyForDish())
        {
            if (_invisibleOnDrag != null) _invisibleOnDrag.active = true;
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<PullableFood>());
            Destroy(copy.GetComponent<Collider>());
            copy.transform.SetParent(this.transform);
            return copy;
        }
        else
        {
            _currentDragPullable = Instantiate(_pullables[0]);
            _currentDragPullable.transform.SetParent(this.transform);
            _currentDragPullable.transform.localScale = _pullables[0].transform.localScale;
            Destroy(_pullables[0]);
            _pullables.RemoveAt(0);
            Notify(new PullablePulledEvent(this));
            return _currentDragPullable;
        }
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        DropCurrentPullable();
    }

    public void OnDragDropFailed(Vector3 position)
    {
        DropCurrentPullable();
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
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

    #endregion
}
