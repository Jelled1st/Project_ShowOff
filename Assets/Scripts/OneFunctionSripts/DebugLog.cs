using UnityEngine;

public class DebugLog : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Write(string output)
    {
        Debug.Log(output);
    }

    public static void WriteStatic(string output)
    {
        Debug.Log(output);
    }
}