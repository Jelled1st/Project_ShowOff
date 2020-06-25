using UnityEngine;
using UnityEngine.EventSystems;

public struct ControllerHitInfo
{
    public ControllerHitInfo(IControllable controllable, RaycastHit hit)
    {
        this.controllable = controllable;
        gameObject = hit.transform.gameObject;
        point = hit.point;
        normal = hit.normal;
        mousePosition = Input.mousePosition;
        uiElement = false;
    }

    public ControllerHitInfo(IControllable controllable, RaycastResult hit)
    {
        this.controllable = controllable;
        gameObject = hit.gameObject;
        point = hit.worldPosition;
        normal = hit.worldNormal;
        mousePosition = Input.mousePosition;
        uiElement = true;
    }

    public ControllerHitInfo(bool everythingNull)
    {
        if (!everythingNull) throw new System.Exception("ControllerHitInfo Everything null error");
        controllable = null;
        gameObject = null;
        point = new Vector3();
        normal = new Vector3();
        mousePosition = new Vector3();
        uiElement = false;
    }

    public readonly IControllable controllable;
    public readonly GameObject gameObject;
    public readonly Vector3 point;
    public readonly Vector3 normal;
    public readonly Vector3 mousePosition;
    public readonly bool uiElement;
}