using UnityEngine;

public class DebugLog : MonoBehaviour
{
    public void Write(string output)
    {
        Debug.Log(output);
    }

    public static void WriteStatic(string output)
    {
        Debug.Log(output);
    }
}