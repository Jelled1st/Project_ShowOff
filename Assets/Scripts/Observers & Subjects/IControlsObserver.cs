using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlsObserver : IObserver
{
    void OnPress(RaycastHit hit);
    void OnHold(float holdTime, RaycastHit hit);
    void OnHoldRelease(float timeHeld, IControllable released);
    void OnSwipe(Vector3 direction, Vector3 lastPoint, RaycastHit hit);
}
