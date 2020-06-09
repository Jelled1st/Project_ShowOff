using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FryingPan : MonoBehaviour, IControllable
{
    [SerializeField] List<GameObject> _foodNodes;
    [SerializeField] Stove stove;
    bool _stoveOn = false;
    List<GameObject> _availableNodes;
    List<BakableFood> _food = new List<BakableFood>();
    Dictionary<BakableFood, GameObject> _foodNodePair = new Dictionary<BakableFood, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _availableNodes = new List<GameObject>(_foodNodes);
    }

    // Update is called once per frame
    void Update()
    {
        if (stove == null || stove.IsOn())
        {
            for (int i = 0; i < _food.Count; ++i)
            {
                _food[i].Bake();
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
    }

    public void RemoveFood(BakableFood food)
    {
        if (food == null) return;
        _availableNodes.Add(_foodNodePair[food]);
        _foodNodePair.Remove(food);
        _food.Remove(food);
        food.fryingPan = null;
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
}
