using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FryingPan : MonoBehaviour, IControllable, ISubject
{
    [SerializeField]
    private List<GameObject> _foodNodes;

    private bool _stoveOn = false;
    private List<GameObject> _availableNodes;
    private List<BakableFood> _food = new List<BakableFood>();
    private Dictionary<BakableFood, GameObject> _foodNodePair = new Dictionary<BakableFood, GameObject>();
    private Dictionary<BakableFood, bool> _foodBaked = new Dictionary<BakableFood, bool>();
    private Dictionary<BakableFood, bool> _foodBurnt = new Dictionary<BakableFood, bool>();

    private List<IObserver> _observers = new List<IObserver>();

    // Start is called before the first frame update
    private void Start()
    {
        _availableNodes = new List<GameObject>(_foodNodes);
    }

    // Update is called once per frame
    private void Update()
    {
        for (var i = 0; i < _food.Count; ++i)
        {
            _food[i].Bake();
            if (_food[i].IsBaked() && !_foodBaked[_food[i]])
            {
                Notify(new BakingDoneEvent(this, _food[i]));
                _foodBaked[_food[i]] = true;
            }
        }
    }

    public void AddFood(BakableFood food)
    {
        if (_availableNodes.Count == 0) return;
        if (_food.Contains(food)) return;
        _food.Add(food);
        _foodNodePair.Add(food, _availableNodes[0]);
        food.transform.position = _availableNodes[0].transform.position;
        _availableNodes.RemoveAt(0);
        food.fryingPan = this;
        _foodBaked.Add(food, false);
        _foodBurnt.Add(food, false);
        Notify(new BakingStartEvent(this, food));
    }

    public void RemoveFood(BakableFood food)
    {
        if (food == null) return;
        _availableNodes.Add(_foodNodePair[food]);
        _foodNodePair.Remove(food);
        _foodBaked.Remove(food);
        _foodBurnt.Remove(food);
        _food.Remove(food);
        food.fryingPan = null;
        Notify(new BakingStopEvent(this, food));
    }

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
        if (dropped is BakableFood)
        {
            AddFood(dropped as BakableFood);
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
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}