using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    void OnPress();
    void OnHold(float holdTime);
    void OnSwipe(Vector3 startPosition, Vector3 endPosition);
}
