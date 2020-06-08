using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T NullIfEqualsNull<T>(this T unityObject) where T : UnityEngine.Object
    {
        return !(unityObject == null || unityObject.Equals(null)) ? unityObject : null;
    }

    public static float Abs(this float value)
    {
        return Mathf.Abs(value);
    }

    public static void ToggleChildren<T>(this Transform transform, bool toggle) where T : IToggleable
    {
        foreach (var toggleable in transform.GetComponentsInChildren<T>())
        {
            if (toggle)
                toggleable.Enable();
            else
                toggleable.Disable();
        }
    }

    public static void ToggleAll<T>(this IEnumerable<T> transform, bool toggle) where T : IToggleable
    {
        foreach (var toggleable in transform)
        {
            if (toggle)
                toggleable.Enable();
            else
                toggleable.Disable();
        }
    }
}