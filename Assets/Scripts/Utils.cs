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

    public static void ToggleChildren<T>(this Transform transform, bool toggle) where T : MonoBehaviour
    {
        transform.GetComponentsInChildren<T>().ToggleAll(toggle);
    }

    public static void ToggleAll<T>(this IEnumerable<T> components, bool toggle) where T : MonoBehaviour
    {
        foreach (var component in components)
        {
            component.enabled = toggle;
        }
    }
}