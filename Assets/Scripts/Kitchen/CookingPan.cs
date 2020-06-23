using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class CookingPan : MonoBehaviour, IControllable, ISubject
{
    [SerializeField] GameObject _foodNode;
    [SerializeField] GameObject _water;
    [SerializeField] GameObject _spoon;
    [SerializeField] GameObject _spoonBottomNode;
    [SerializeField] float _stirBonusModifier = 0.5f;
    [SerializeField] float _stirTime = 0.8f;
    Animator _spoonAnimator;
    private List<CookableFood> _food = new List<CookableFood>();
    private bool _foodIsCooked = false;

    List<IObserver> _observers = new List<IObserver>();


    // Start is called before the first frame update
    void Start()
    {
        _spoonAnimator = _spoon.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_food.Count == 0) return;
        CookAllFood();
        if (_food[_food.Count - 1].IsCooked() && !_foodIsCooked)
        {
            _foodIsCooked = true;
            Notify(new CookingDoneEvent(this, _food[_food.Count - 1]));
            if (_food[_food.Count - 1].IsCooked(true)) Notify(new CookingAllIngredientsDoneEvent(this));
        }
        _water.GetComponent<Renderer>().material.SetFloat("_OilCooking", _food[_food.Count-1].GetProgress());
    }

    private void CookAllFood(float mod = 1.0f)
    {
        for (int i = 0; i < _food.Count; ++i) _food[i].Cook(mod);
    }

    public void TryAddFood(CookableFood food)
    {
        List<CookableFood> requiredFood = food.GetRequiredHeadIngredients();
        if (requiredFood.Count != 0)
        {
            bool containsIngredient = false;
            for(int i = 0; i < requiredFood.Count; ++i)
            {
                if(requiredFood[i] == null || _food.Contains(requiredFood[i]))
                {
                    containsIngredient = true;
                    break;
                }
            }
            if(!containsIngredient || !_food[_food.Count -1].IsCooked(false)) return;
        }
        _food.Add(food);
        food.transform.SetParent(_foodNode.transform);
        food.transform.localPosition = new Vector3(0, 0, 0);
        food.cookingPan = this;
        Notify(new CookingStartEvent(this, food));
        _foodIsCooked = false;
    }

    public void RemoveFood(CookableFood food)
    {
        if (_food.Remove(food))
        {
            Notify(new CookingStopEvent(this, food));
            food.cookingPan = null;
        }
    }

    #region IControllable
    public GameObject GetDragCopy()
    {
        if (_food == null || _food.Count == 0 || !_food[_food.Count - 1].IsCooked(true))
        {
            return null;
        }
        GameObject copy = Instantiate(_spoon);
        copy.transform.localScale = _spoon.transform.lossyScale;
        Animator ani = copy.GetComponent<Animator>();
        Destroy(ani);
        GameObject empty = new GameObject();
        empty.transform.SetParent(copy.transform);
        empty.transform.localScale = new Vector3(
                empty.transform.localScale.x * _spoonBottomNode.transform.localScale.x,
                empty.transform.localScale.y * _spoonBottomNode.transform.localScale.y,
                empty.transform.localScale.z * _spoonBottomNode.transform.localScale.z
                );

        empty.transform.localPosition = _spoonBottomNode.transform.localPosition;
        for (int i = 0; i < _food.Count; ++i)
        {
            GameObject foodCopy = _food[i].GetDragCopy();
            foodCopy.transform.SetParent(empty.transform);
            foodCopy.transform.localPosition = new Vector3(0, 0, 0);
            foodCopy.transform.localScale = _food[i].transform.lossyScale;
        }
        Destroy(copy.GetComponent<CookingPan>());
        Collider[] colliders = copy.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; ++i)
        {
            Destroy(colliders[i]);
        }
        return copy;
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        if (_food != null && _food.Count > 0)
        {
            for (int i = _food.Count - 1; i >= 0; --i)
            {
                CookableFood food = _food[i];
                food.OnDragDrop(position, droppedOn, hitInfo);
                droppedOn.OnDrop(food.GetComponent<IControllable>(), hitInfo);
            }
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if (dropped is CookableFood)
        {
            TryAddFood(dropped as CookableFood);
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
        if (!_spoonAnimator.GetCurrentAnimatorStateInfo(0).IsName("anim_spoon_stirring"))
        {
            _spoonAnimator.SetTrigger("isPlaying");
            _water.transform.DORotate(new Vector3(0, 360, 0), _stirTime, RotateMode.LocalAxisAdd);
            _foodNode.transform.DORotate(new Vector3(0, 360, 0), _stirTime, RotateMode.LocalAxisAdd);
            CookAllFood(_stirBonusModifier);
            Notify(new CookingStirEvent(this));
        }
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
