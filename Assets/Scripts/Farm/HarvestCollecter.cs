using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestCollecter : MonoBehaviour, IControllable
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void OnClick(Vector3 hitPoint)
    {
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
        if(dropped is FarmPlot)
        {
            FarmPlot plot = (FarmPlot)dropped;
            Debug.Log("plot dropped");
            plot.Harvest();
        }
    }

    public GameObject GetDragCopy()
    {
        return null;
    }
}
