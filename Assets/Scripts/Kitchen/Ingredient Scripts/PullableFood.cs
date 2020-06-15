using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullableFood : MonoBehaviour, IIngredient, IControllable, ISubject
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private List<PullableFoodPullable> _pullables;
    [SerializeField] private InvisibleOnDrag _invisibleOnDrag;

    private List<IObserver> _observers = new List<IObserver>();

    void Awake()
    {
        this.tag = "Ingredient";
    }

    void Start()
    {
        if(_invisibleOnDrag != null) _invisibleOnDrag.active = false;
        for(int i = 0; i < _pullables.Count; ++i)
        {
            _pullables[i].foodParent = this;
        }
    }

    public void Pull(PullableFoodPullable pulled)
    {
        if (_pullables.Contains(pulled))
        {
            _pullables.Remove(pulled);
        }
    }

    #region IIngredient
    public void AddedToDish()
    {
        Destroy(this.gameObject);
    }

    public GameObject GetDishMesh()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<PullableFood>());
        Destroy(copy.GetComponent<Collider>());
        copy.transform.localScale = this.transform.lossyScale;
        copy.GetComponent<Renderer>().enabled = true;
        return copy;
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

    #region IControllable

    public GameObject GetDragCopy()
    {
        if(ReadyForDish())
        {
            if (_invisibleOnDrag != null) _invisibleOnDrag.active = true;
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<PullableFood>());
            Destroy(copy.GetComponent<Collider>());
            copy.transform.localScale = this.transform.lossyScale;
            return copy;
        }
        return null;
    }

    public void OnClick(Vector3 hitPoint)
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
