using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Dish : MonoBehaviour, IControllable, ISubject, IDishObserver
{
    [Header("Required Ingredients")]
    [SerializeField] private List<IngredientType> _requiredIngredients;
    [Tooltip("The placements of the required ingredients")]
    [SerializeField] private List<GameObject> _requiredPlacements;
    [Tooltip("True means that the index in the placement list corrosponds to the index in the ingredient list")]
    [SerializeField] private bool _placeRequiredInOrder;

    [Header("Optional Ingredients")]
    [SerializeField] private List<IngredientType> _optionalIngredients;
    [Tooltip("The placements of the optional ingredients")]
    [SerializeField] private List<GameObject> _optionalPlacements;
    [Tooltip("True means that the index in the placement list corrosponds to the index in the ingredient list")]
    [SerializeField] private bool _placeOptionalInOrder;

    [Header("Misc")]
    [Tooltip("the dishes this dish is dependent on. All dependent dishes must be done before completing this")]
        [SerializeField] List<Dish> _sideDishesLeft = new List<Dish>();
    [Tooltip("Stack all ingredients from the using the required placements index 0")]
        [SerializeField] private bool _stackAllIngredients = false;
    private List<IngredientType> _addedIngredients = new List<IngredientType>();
    private Dictionary<IngredientType, GameObject> _addedIngredientObjects = new Dictionary<IngredientType, GameObject>();

    private List<IDishObserver> _observers = new List<IDishObserver>();

    private bool _debugLog = false;

    public void Awake()
    {
        this.gameObject.tag = "Dish"; 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_stackAllIngredients)
        {
            if(_requiredPlacements.Count > 0)
            {
                GameObject placement = _requiredPlacements[0];
                _requiredPlacements = new List<GameObject>();
                _requiredPlacements.Add(placement);
                _optionalPlacements = null;
            }
        }
        else
        {
            if (_requiredIngredients.Count != _requiredPlacements.Count)
                Debug.Log("Burgerdish warning: Amount of required ingredients does not match placement options");
            if (_optionalIngredients.Count != _optionalIngredients.Count)
                Debug.Log("Burgerdish warning: Amount of optional ingredients does not match placement options");
        }

        for(int i = 0; i < _sideDishesLeft.Count; ++i)
        {
            Subscribe(_sideDishesLeft[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool TryAddIngredient(IIngredient ingredient)
    {
        if (_debugLog) Debug.Log("Trying to add");
        IngredientType type = ingredient.GetIngredientType();
        for (int i = 0; i < _requiredIngredients.Count; ++i)
        {
            if (type == _requiredIngredients[i])
            {
                if (_debugLog) Debug.Log("Required ingredient");
                //required ingredient
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight(), true, i);
                _requiredIngredients.RemoveAt(i); //remove ingredient from the list
                InformObserversAddIngredient(ingredient);
                if (_requiredIngredients.Count == 0)
                {
                    if(_sideDishesLeft.Count == 0) InformObserversFinish();
                }
                return true;
            }
        }
        for (int i = 0; i < _optionalIngredients.Count; ++i)
        {
            if (type == _optionalIngredients[i])
            {
                if (_debugLog) Debug.Log("Optional ingredient");
                //required ingredient
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight(), false, i);
                _optionalIngredients.RemoveAt(i); //remove ingredient from the list
                InformObserversAddIngredient(ingredient);
                return true;
            }
        }
        if (_debugLog) Debug.Log("Could not be added");
        return false;
    }

    private void AddIngredientMesh(IngredientType type, GameObject ingredientMesh, float ingredientHeight, bool requiredIngredient, int indexInList)
    {
        if (ingredientMesh == null) return;
        _addedIngredients.Add(type);
        GameObject ingredientGO = ingredientMesh;
        _addedIngredientObjects.Add(type, ingredientGO);
        ingredientGO.transform.SetParent(this.transform);

        //saving the upcoming 4 vars makes sure the code only requires 2 if-else statements  
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        List<GameObject> placementList;
        bool inOrder = false;
        if (_stackAllIngredients)
        {
            pos = _requiredPlacements[0].transform.position;
            rot = _requiredPlacements[0].transform.rotation;
            _requiredPlacements[0].transform.position += new Vector3(0, ingredientHeight, 0);
        }
        else
        {
            if (requiredIngredient)
            {
                placementList = _requiredPlacements;
                if (_placeRequiredInOrder) inOrder = true;
            }
            else
            {
                placementList = _optionalPlacements;
                if (_placeOptionalInOrder) inOrder = true;
            }


            if (inOrder)
            {
                pos = placementList[indexInList].transform.position;
                rot = placementList[indexInList].transform.rotation;
                _requiredPlacements.RemoveAt(indexInList);
            }
            else
            {
                int rand = Random.Range(0, _requiredPlacements.Count);
                pos = placementList[rand].transform.position;
                rot = placementList[rand].transform.rotation;
                _requiredPlacements.RemoveAt(rand);
            }
        }
        ingredientGO.transform.position = pos;
        ingredientGO.transform.rotation = rot;
    }

    #region IControllable
    public GameObject GetDragCopy()
    {
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
        if (dropped is IIngredient)
        {
            IIngredient ingredient = dropped as IIngredient;
            if (ingredient.ReadyForDish()) if (TryAddIngredient(ingredient)) ingredient.AddedToDish();
        }
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
        Debug.Log("Finished: " + (_requiredIngredients.Count == 0 && _sideDishesLeft.Count == 0));
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }
    #endregion

    #region ISubject
    public void Register(IObserver observer)
    {
        if (observer is IDishObserver)
        {
            _observers.Add(observer as IDishObserver);
        }
    }

    public void UnRegister(IObserver observer)
    {
        if (observer is IDishObserver)
        {
            //_observers.Remove(observer as IDishObserver);
        }
    }

    public void Notify(AObserverEvent observerEvent)
    {
    }
    #endregion

    #region IDishObserver
    private void InformObserversAddIngredient(IIngredient ingredient)
    {
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnIngredientAdd(this, ingredient);
        }
    }

    private void InformObserversFinish()
    {
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnFinishDish(this);
        }
    }

    public void OnIngredientAdd(ISubject subject, IIngredient ingredient)
    {
    }

    public void OnFinishDish(ISubject subject)
    {
        Dish dish = subject as Dish;
        if (dish != null) _sideDishesLeft.Remove(dish);
        if (_requiredIngredients.Count == 0 && _sideDishesLeft.Count == 0) InformObserversFinish();
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
