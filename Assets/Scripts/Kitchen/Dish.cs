using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Dish : MonoBehaviour, IControllable, ISubject, IDishObserver
{
    public enum DishTypes
    {
        Undifined = -1,
        BurgerAndFries = 0,
        ChiliCheeseFries,
        FishAndChips,
        SideDish,
    }

    [SerializeField] DishTypes _dishType;

    [Header("Required Ingredients")]
    [SerializeField] protected List<IngredientType> _requiredIngredients;
    [SerializeField] protected List<GameObject> _requiredIngredientMeshes;

    [Header("Optional Ingredients")]
    [SerializeField] protected List<IngredientType> _optionalIngredients;
    [Tooltip("The placements of the optional ingredients")]
    [SerializeField] protected List<GameObject> _optionalIngredientMeshes;

    [Header("Final ingredient")]
    [SerializeField] protected IngredientType _finishIngredient;
    protected bool _finishIngredientPlaced = false;
    [SerializeField] protected GameObject _finishIngredientMesh;

    [Header("Misc")]
    [Tooltip("the dishes this dish is dependent on. All dependent dishes must be done before completing this")]
    [SerializeField] List<Dish> _sideDishesLeft = new List<Dish>();
    [Tooltip("Auto finish is that the player does not need to make an action to register a complete side dish, if false, the player needs to drag it to this")]
    [SerializeField] List<bool> _sideDishesAutoComplete = new List<bool>();
    [SerializeField] List<GameObject> _sideDishGameMeshes = new List<GameObject>();
    [Tooltip("Stack all ingredients from the using the required placements index 0")]
    [SerializeField] protected bool _stackAllIngredients = false;
    [SerializeField] protected GameObject _stackIngredientsNode;
    protected List<IngredientType> _addedIngredients = new List<IngredientType>();
    protected Dictionary<IngredientType, GameObject> _addedIngredientObjects = new Dictionary<IngredientType, GameObject>();

    protected List<IDishObserver> _observers = new List<IDishObserver>();

    protected bool _debugLog = false;

    protected void Awake()
    {
        this.gameObject.tag = "Dish"; 
    }

    // Start is called before the first frame update
    protected void Start()
    {
        if (!_stackAllIngredients)
        {
            if (_requiredIngredients.Count != _requiredIngredientMeshes.Count)
                Debug.Log("Dish warning: Amount of required ingredients does not match meshes count");
            if (_optionalIngredients.Count != _optionalIngredientMeshes.Count)
                Debug.Log("Dish warning: Amount of optional ingredients does not match meshes count");
            if (_sideDishesLeft.Count != _sideDishesAutoComplete.Count || _sideDishesLeft.Count != _sideDishGameMeshes.Count)
                Debug.Log("Side dishes count does not equal side dish auto complete or side dish mesh count");
        }

        for(int i = 0; i < _sideDishesLeft.Count; ++i)
        {
            Subscribe(_sideDishesLeft[i]);
        }
    }

    // Update is called once per frame
    protected void Update()
    {

    }

    public DishTypes GetDishType()
    {
        return _dishType;
    }

    public List<IngredientType> GetRequiredIngredients()
    {
        return _requiredIngredients;
    }

    public List<IngredientType> GetOptionalIngredients()
    {
        return _optionalIngredients;
    }

    public IngredientType GetFinishIngredient()
    {
        return _finishIngredient;
    }

    public List<Dish> GetSideDishesLeft()
    {
        return _sideDishesLeft;
    }

    public List<IngredientType> GetAllPossibleIngredients()
    {
        List<IngredientType> allIngredients = new List<IngredientType>(_requiredIngredients);
        for(int i = 0; i < _optionalIngredients.Count; ++i)
        {
            allIngredients.Add(_optionalIngredients[i]);
        }
        if (_finishIngredient != IngredientType.Undefined) allIngredients.Add(_finishIngredient);
        for(int i = 0; i < _sideDishesLeft.Count; ++i)
        {
            List<IngredientType> ingredientTypes = _sideDishesLeft[i].GetAllPossibleIngredients();
            for (int j = 0; j < ingredientTypes.Count; ++j)
            {
                allIngredients.Add(ingredientTypes[j]);
            }
        }
        return allIngredients;
    }

    protected virtual bool TryAddIngredient(IIngredient ingredient)
    {
        if (_debugLog) Debug.Log("Trying to add");
        IngredientType type = ingredient.GetIngredientType();
        if(_finishIngredient != IngredientType.Undefined && type == _finishIngredient)
        {
            if (IsFinished(false))
            {
                if (_debugLog) Debug.Log("Finish ingredient");
                //required ingredient
                AddFinalIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight());
                InformObserversAddIngredient(ingredient);
                _finishIngredientPlaced = true;
                InformObserversFinish();
                return true;
            }
        }
        if (_finishIngredientPlaced) return false;
        for (int i = 0; i < _requiredIngredients.Count; ++i)
        {
            if (type == _requiredIngredients[i])
            {
                if (_debugLog) Debug.Log("Required ingredient");
                //required ingredient
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight(), true, i);
                _requiredIngredients.RemoveAt(i); //remove ingredient from the list
                InformObserversAddIngredient(ingredient);
                if(IsFinished(true)) InformObserversFinish();
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

    protected virtual bool IsFinished(bool includeFinishIngredient)
    {
        if (_requiredIngredients.Count == 0)
        {
            if (_sideDishesLeft.Count == 0)
            {
                if (!includeFinishIngredient) return true;
                else if (_finishIngredient == IngredientType.Undefined) return true;
                else if (_finishIngredientPlaced) return true;
            }
        }
        return false;
    }

    protected void AddIngredientMesh(IngredientType type, GameObject ingredientMesh, float ingredientHeight, bool requiredIngredient, int indexInList)
    {
        if (ingredientMesh == null) return;
        if(_addedIngredientObjects.ContainsKey(type))
        {
            Destroy(ingredientMesh);
            return;
        }
        _addedIngredients.Add(type);

        if (_stackAllIngredients)
        {
            _addedIngredientObjects.Add(type, ingredientMesh);
            ingredientMesh.transform.SetParent(this.transform);

            Vector3 pos = new Vector3();
            Quaternion rot = new Quaternion();
            pos = _stackIngredientsNode.transform.position;
            rot = _stackIngredientsNode.transform.rotation;
            _stackIngredientsNode.transform.position += new Vector3(0, ingredientHeight, 0);
            ingredientMesh.transform.position = pos;
            ingredientMesh.transform.rotation = rot;
        }
        else
        {
            Destroy(ingredientMesh);
            if(requiredIngredient)
            {
                if (_requiredIngredientMeshes[indexInList] != null)
                {
                    _requiredIngredientMeshes[indexInList].SetActive(true);
                    _addedIngredientObjects.Add(type, _requiredIngredientMeshes[indexInList]);
                    _requiredIngredientMeshes.RemoveAt(indexInList);
                }
            }
            else
            {
                if (_optionalIngredientMeshes[indexInList] != null)
                {
                    _optionalIngredientMeshes[indexInList].SetActive(true);
                    _addedIngredientObjects.Add(type, _optionalIngredientMeshes[indexInList]);
                    _optionalIngredientMeshes.RemoveAt(indexInList);
                }
            }
        }
    }

    protected void AddFinalIngredientMesh(IngredientType type, GameObject ingredientMesh, float ingredientHeight)
    {
        if (ingredientMesh == null) return;
        _addedIngredients.Add(type);

        if (_stackAllIngredients)
        {
            GameObject ingredientGO = ingredientMesh;
            _addedIngredientObjects.Add(type, ingredientGO);
            ingredientGO.transform.SetParent(this.transform);
            Vector3 pos = new Vector3();
            Quaternion rot = new Quaternion();
            pos = _stackIngredientsNode.transform.position;
            rot = _stackIngredientsNode.transform.rotation;
            _stackIngredientsNode.transform.position += new Vector3(0, ingredientHeight, 0);
        }
        else
        {
            Destroy(ingredientMesh);
            _finishIngredientMesh.SetActive(true);
            _addedIngredientObjects.Add(type, _finishIngredientMesh);
            _finishIngredientMesh = null;
        }
    }

    #region IControllable
    public virtual GameObject GetDragCopy()
    {
        if (IsFinished(true) && _dishType == DishTypes.SideDish)
        {
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<Dish>());
            Destroy(copy.GetComponent<Collider>());
            return copy;
        }
        else return null;
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
        else if(dropped is Dish)
        {
            Dish dish = dropped as Dish;
            for (int i = 0; i < _sideDishesLeft.Count; ++i)
            {
                if (_sideDishesLeft[i] as Dish == dish)
                {
                    Destroy(dish.gameObject);
                    _sideDishesLeft.RemoveAt(i);
                    _sideDishGameMeshes[i]?.SetActive(true);
                }
            }
            if (IsFinished(true))
            {
                InformObserversFinish();
            }
        }
    }

    public virtual void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public virtual void OnPress(Vector3 hitPoint)
    {
        Debug.Log("Finished: " + IsFinished(true));
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
            _observers.Remove(observer as IDishObserver);
        }
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
    #endregion

    #region IDishObserver
    protected void InformObserversAddIngredient(IIngredient ingredient)
    {
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnIngredientAdd(this, ingredient);
        }
    }

    protected void InformObserversFinish()
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
        if (dish != null)
        {
            for (int i = 0; i < _sideDishesLeft.Count; ++i)
            {
                if (_sideDishesLeft[i] == dish)
                {
                    if (_sideDishesAutoComplete[i])
                    {
                        _sideDishesLeft.RemoveAt(i);
                        _sideDishGameMeshes[i]?.SetActive(true);
                    }
                }
            }
        }
        if (_requiredIngredients.Count == 0 && _sideDishesLeft.Count == 0)
        {
            InformObserversFinish();
        }
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
