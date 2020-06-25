using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CookableFood : MonoBehaviour, IControllable, IIngredient
{
    [SerializeField]
    private IngredientType _ingredientType;

    [SerializeField]
    private float _ingredientHeight;

    [SerializeField]
    private float _timeToCook = 2.0f;

    [HideInInspector]
    public CookingPan cookingPan;

    [SerializeField]
    private List<CookableFood> _completeCookingWith;

    [SerializeField]
    private List<CookableFood> _canOnlyBeAddedWithFood;

    [SerializeField]
    private GameObject _currentMesh;

    [SerializeField]
    private GameObject _cookMesh;

    [SerializeField]
    private Vector3 _cookMeshOffset;

    private bool _stateCook = false;
    private float _cookedTime = 0.0f;
    private bool _isDoneCooking = false;

    private List<IObserver> _observers = new List<IObserver>();

    private void Awake()
    {
        gameObject.tag = "Ingredient";
    }

    public void Cook(float modifier = 1.0f)
    {
        if (!_stateCook)
        {
            var transform = _currentMesh.transform;
            _stateCook = true;
            Destroy(_currentMesh);
            _currentMesh = Instantiate(_cookMesh);
            _currentMesh.transform.SetParent(this.transform);
            _currentMesh.transform.localPosition = transform.localPosition + _cookMeshOffset;
            _currentMesh.transform.localRotation = transform.localRotation;
        }

        _cookedTime += Time.deltaTime * modifier;
        if (!_isDoneCooking && IsCooked(true))
        {
            _isDoneCooking = true;
            Notify(new IngredientDoneEvent(this));
        }
    }

    public bool IsCooked(bool withSideIngredients = false)
    {
        if (_isDoneCooking) return true;
        if (!withSideIngredients)
        {
            return _cookedTime >= _timeToCook;
        }
        else
        {
            if (_cookedTime < _timeToCook) return false;
            var allDone = true;
            for (var i = 0; i < _completeCookingWith.Count; ++i)
            {
                if (!_completeCookingWith[i].IsCooked(true))
                {
                    allDone = false;
                    break;
                }
            }

            return allDone;
        }
    }

    public List<CookableFood> GetRequiredHeadIngredients()
    {
        return _canOnlyBeAddedWithFood;
    }

    public float GetProgress()
    {
        return Mathf.Min(_cookedTime / _timeToCook, 1);
    }


    #region IIngredient

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return IsCooked(true);
    }

    public void AddedToDish()
    {
        cookingPan?.RemoveFood(this);
        Destroy(gameObject);
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public GameObject GetDishMesh()
    {
        if (ReadyForDish())
        {
            var copy = Instantiate(gameObject);
            Destroy(copy.GetComponent<CookableFood>());
            Destroy(copy.GetComponent<Collider>());
            var renderers = copy.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }

            return copy;
        }

        return null;
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

    public GameObject GetDragCopy()
    {
        var copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<CookableFood>());
        Destroy(copy.GetComponent<Collider>());
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