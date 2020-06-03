public static class Utils
{
    public static T NullIfEqualsNull<T>(this T unityObject) where T : UnityEngine.Object
    {
        return !unityObject.Equals(null) ? unityObject : null;
    }
}