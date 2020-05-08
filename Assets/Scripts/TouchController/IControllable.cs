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
}
