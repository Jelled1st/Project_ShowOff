using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    //OnClick is called everytime a clicked mouse hits the object
    void OnClick(Vector3 hitPoint);


    //OnPress is called when the object is pressed (the mouse is not held, a single press)
    void OnPress(Vector3 hitPoint);


    //OnHold is called when the mouse is clicked on the object and held
    void OnHold(float holdTime, Vector3 hitPoint);
    //OnHoldRelease is called when the mouse let's go of the hold on the object
    void OnHoldRelease(float timeHeld);


    //OnSwipe is called when a swipe hits the object
    void OnSwipe(Vector3 direction, Vector3 lastPoint);
    // OnDrag called every frame that an object is dragged


    void OnDrag(Vector3 position);
    // OnDragDrop is called when the object is released from dragging
    // and the drop was succesful - it was dropped on an IControllable
    void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo);
    // OnDragDropFailed is called when the object is released from dragging
    // but the drop failed - it was dropped in empty space
    void OnDragDropFailed(Vector3 position);
    // OnDrop is called when something is dropped on the object
    void OnDrop(IControllable dropped, ControllerHitInfo hitInfo);

    GameObject GetDragCopy();
}
