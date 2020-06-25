using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour, IControllable, ISubject
{
    [SerializeField]
    private GameObject _cutPosition;

    private CuttableFood _selected = null;

    private List<IObserver> _observers = new List<IObserver>();

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Vector3 GetCutPosition()
    {
        return _cutPosition.transform.position;
    }

    public void RequestRemoveSelected(CuttableFood food)
    {
        if (_selected == food) _selected = null;
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
        if (dropped is CuttableFood && _selected == null)
        {
            var food = dropped as CuttableFood;
            _selected = food;
            food.cuttingBoard = this;
            food.transform.position = _cutPosition.transform.position;
            food.transform.rotation = _cutPosition.transform.rotation;
            Notify(new CuttableOnCuttingBoardEvent(this, food));
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