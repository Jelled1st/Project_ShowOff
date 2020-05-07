using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    void OnPress(Vector3 hitPoint);
    void OnHold(float holdTime, Vector3 hitPoint);
    void OnHoldRelease(float timeHeld);
    void OnSwipe(Vector3 direction, Vector3 lastPoint);
}
