using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmUnit : MonoBehaviour, IControllable
{
    private Swarm _swarm;

    public void Init(Swarm swarm)
    {
        _swarm = swarm;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("on trigger enter");
        FarmPlot plot;
        if (other.gameObject.TryGetComponent<FarmPlot>(out plot))
        {
            _swarm.UnitReachedPlot(this, plot);
        }
    }

    public void OnClick(Vector3 hitPoint)
    {
        Debug.Log("unit hit and " + _swarm);
        _swarm.UnitHit(this);
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
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
    }

    public GameObject GetDragCopy()
    {
        return null;
    }
}
