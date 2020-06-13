using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PullableFoodPullable : MonoBehaviour, IControllable
{
    [HideInInspector] public PullableFood foodParent;

    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<PullableFoodPullable>());
        Destroy(copy.GetComponent<Collider>());
        copy.transform.localScale = this.transform.lossyScale;
        return copy;
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
        foodParent.Pull(this);
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        Destroy(this.gameObject);
    }

    public void OnDragDropFailed(Vector3 position)
    {
        Destroy(this.gameObject);
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
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
}
