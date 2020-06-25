using UnityEngine;

public class FPSDebug : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private int[] frameTimes;

    private void Start()
    {
        frameTimes = new int[100];
    }

    private void Update()
    {
        //deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        deltaTime = Time.unscaledDeltaTime;
        AddFrameTime(Mathf.RoundToInt(deltaTime * 1000));
    }

    private void AddFrameTime(int time)
    {
        for (var i = 0; i < frameTimes.Length - 1; ++i)
        {
            frameTimes[i] = frameTimes[i + 1];
        }

        frameTimes[frameTimes.Length - 1] = time;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        var style = new GUIStyle();

        var rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        var msec = deltaTime * 1000.0f;
        var fps = 1.0f / deltaTime;
        var text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);

        /*
        int width = 2;
        Rect debuggerRect = new Rect(w-width*frameTimes.Length, 0, width * frameTimes.Length, 100);
        Texture2D frameTimeRect = new Texture2D((int)debuggerRect.width, (int)debuggerRect.height);

        for (int i = 0; i < frameTimes.Length; ++i)
        {
            for(int y = 0; y < frameTimes[i]; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    frameTimeRect.SetPixel((width*i)+x, y, new Color((frameTimes[i]/125), (255-frameTimes[i]/255), 0));
                }
            }
        }
        frameTimeRect.Apply();
        GUI.skin.box.normal.background = frameTimeRect;
        GUI.Box(debuggerRect, GUIContent.none);
        */
    }
}