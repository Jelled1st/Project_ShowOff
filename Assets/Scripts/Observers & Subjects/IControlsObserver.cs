using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Observes the TouchController, everytime a method is called on an IControllable,
//it will also be called on this
public interface IControlsObserver : IObserver
{
    //OnClick is called everytime a clicked mouse hits the object
    void OnClick(ControllerHitInfo hitInfo);
    //OnPress is called when the object is pressed (the mouse is not held, a single press)
    
    
    void OnPress(ControllerHitInfo hitInfo);
    //OnHold is called when the mouse is clicked on the object and held
    
    
    void OnHold(float holdTime, ControllerHitInfo hitInfo);
    //OnHoldRelease is called when the mouse let's go of the hold on the object
    void OnHoldRelease(float timeHeld, IControllable released);
    //OnSwipe is called when a swipe hits the object
    
    
    void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo);


    void OnDrag(Vector3 position, IControllable dragged);
    // OnDragDrop is called when the object is released from dragging
    // and the drop was succesful - it was dropped on an IControllable
    void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo);
    // OnDragDropFailed is called when the object is released from dragging
    // but the drop failed - it was dropped in empty space
    void OnDragDropFailed(Vector3 position, IControllable dragged);
}
