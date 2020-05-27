using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour, IControllable
{
    [SerializeField] private GameObject _cutPosition;
    private CuttableFood _selected = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        if(dropped is CuttableFood && _selected == null)
        {
            CuttableFood food = dropped as CuttableFood;
            _selected = food;
            food.cuttingBoard = this;
            food.transform.position = _cutPosition.transform.position;
            food.transform.rotation = _cutPosition.transform.rotation;
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
