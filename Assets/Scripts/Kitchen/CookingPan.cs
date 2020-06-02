using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CookingPan : MonoBehaviour, IControllable
{
    [SerializeField] List<GameObject> _foodNodes;
    List<GameObject> _availableNodes;
    List<CookableFood> _food = new List<CookableFood>();
    Dictionary<CookableFood, GameObject> _foodNodePair = new Dictionary<CookableFood, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        _availableNodes = new List<GameObject>(_foodNodes);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _food.Count; ++i)
        {
            _food[i].Cook();
        }
    }

    public void AddFood(CookableFood food)
    {
        if (_availableNodes.Count == 0) return;
        _food.Add(food);
        _foodNodePair.Add(food, _availableNodes[0]);
        food.transform.position = _availableNodes[0].transform.position;
        _availableNodes.RemoveAt(0);
        food.cookingPan = this;
    }

    public void RemoveFood(CookableFood food)
    {
        if (food == null) return;
        _availableNodes.Add(_foodNodePair[food]);
        _foodNodePair.Remove(food);
        _food.Remove(food);
        food.cookingPan = null;
    }

    #region IControllable
    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        for (int i = 0; i < _food.Count; ++i)
        {
            GameObject foodCopy = _food[i].GetDragCopy();
            foodCopy.transform.SetParent(copy.transform);
            //foodCopy.transform.localPosition = new Vector3(0, 0.1f, 0);
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
        // call the ondrop function on the droppedon with every food 
        for (int i = 0; i < _food.Count; ++i)
        {
            droppedOn.OnDrop(_food[i].GetComponent<IControllable>(), hitInfo);
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if (dropped is CookableFood)
        {
            AddFood(dropped as CookableFood);
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
    #endregion
}
