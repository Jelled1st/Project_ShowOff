using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    void OnPress();
    void OnHold(float holdTime);
    void OnHoldRelease(float timeHeld);
    void OnSwipe(Vector3 direction);
}
