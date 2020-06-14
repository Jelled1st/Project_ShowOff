using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class FryFryer : MonoBehaviour, IControllable, ISubject
{
    [SerializeField] private GameObject _basket;
    [SerializeField] GameObject _foodNode;
    [SerializeField] GameObject _basketDownNode;
    [SerializeField] ProgressBar _progressBar;
    FryableFood _food;
    private bool _basketIsUp = true;

    private List<IObserver> _observers = new List<IObserver>();

    // Start is called before the first frame update
    void Start()
    {
        _progressBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(_food != null && !_basketIsUp)
        {
            _food.Fry();
            float value = _food.GetTimeFried() / _food.GetFryTime();
            _progressBar.SetPercentage(value);
            _progressBar.SetFillColor(new Color(1-value, value, 0, 1));
            if (_food.IsFried())
            {
                Notify(new FryerStopEvent(this, _food));
                MoveBasketUp();
            }
        }
    }

    private void MoveBasketUp()
    {
        Vector3 pos = _basketDownNode.transform.position;
        Vector3 rot = _basketDownNode.transform.rotation.eulerAngles;
        if (_food != null)
        {
            Vector3 foodPos = _food.transform.position + pos - _basket.transform.position;
            Vector3 foodRotation = (Quaternion.FromToRotation(_basket.transform.rotation.eulerAngles, _basketDownNode.transform.rotation.eulerAngles) * _food.transform.rotation).eulerAngles;
            _food.transform.DOMove(foodPos, 0.3f);
            _food.transform.DORotate(foodRotation, 0.3f);
        }
        _basketDownNode.transform.position = _basket.transform.position;
        _basketDownNode.transform.rotation = _basket.transform.rotation;
        _basket.transform.DOMove(pos, 0.3f);
        _basket.transform.DORotate(rot, 0.3f);
        _basketIsUp = true;
        _progressBar.SetActive(false);
    }

    private void MoveBasketDown()
    {
        Vector3 pos = _basketDownNode.transform.position;
        Vector3 rot = _basketDownNode.transform.rotation.eulerAngles;
        if (_food != null)
        {
            Vector3 foodPos = _food.transform.position + pos - _basket.transform.position;
            Vector3 foodRotation = (Quaternion.FromToRotation(_basket.transform.rotation.eulerAngles, _basketDownNode.transform.rotation.eulerAngles) * _food.transform.rotation).eulerAngles;
            _food.transform.DOMove(foodPos, 0.3f);
            _food.transform.DORotate(foodRotation, 0.3f);
        }
        _basketDownNode.transform.position = _basket.transform.position;
        _basketDownNode.transform.rotation = _basket.transform.rotation;
        _basket.transform.DOMove(pos, 0.3f);
        _basket.transform.DORotate(rot, 0.3f);

        _basketIsUp = false;
        _progressBar.SetActive(true);
    }

    public void TrySetFood(FryableFood food)
    {
        if (_food != null) return;
        _food = food;
        food.transform.position = _foodNode.transform.position;
        food.fryer = this;
        Notify(new FryerStartEvent(this, food));
        MoveBasketDown();
    }

    public void RemoveFood(FryableFood food)
    {
        if (food == null) return;
        _food = null;
        food.fryer = null;
    }

    #region IControllable
    public GameObject GetDragCopy()
    {
        if (_food == null || !_basketIsUp)
        {
            Debug.Log("Returning null");
            return null;
        }
        GameObject copy = _food.GetDragCopy();
        copy.transform.SetParent(null);
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
        if(_food != null) droppedOn.OnDrop(_food.GetComponent<IControllable>(), hitInfo);
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if (dropped is FryableFood)
        {
            TrySetFood(dropped as FryableFood);
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
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}
