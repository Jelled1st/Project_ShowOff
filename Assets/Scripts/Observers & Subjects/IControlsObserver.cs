using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlsObserver : IObserver
{
    void OnPress(IControllable pressed, Vector3 hitPoint);
    void OnHold(float holdTime, IControllable held, Vector3 hitPoint);
    void OnHoldRelease(float timeHeld, IControllable released);
    void OnSwipe(Vector3 direction, Vector3 lastPoint, IControllable swiped);
}
