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
}