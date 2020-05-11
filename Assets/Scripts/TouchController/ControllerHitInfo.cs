using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public struct ControllerHitInfo
{
    public ControllerHitInfo(IControllable controllable, RaycastHit hit)
    {
        this.controllable = controllable;
        gameObject = hit.transform.gameObject;
        point = hit.point;
        normal = hit.normal;
        uiElement = false;
    }

    public ControllerHitInfo(IControllable controllable, RaycastResult hit)
    {
        this.controllable = controllable;
        gameObject = hit.gameObject;
        point = hit.worldPosition;
        normal = hit.worldNormal;
        uiElement = true;
    }

    public readonly IControllable controllable;
    public readonly GameObject gameObject;
    public readonly Vector3 point;
    public readonly Vector3 normal;
    public readonly bool uiElement;
}
