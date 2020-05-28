using System;
using UnityEngine;
using Timer = System.Timers.Timer;

public static class TimeoutController
{
    public const float TimeoutInterval = 30f;

    public static event Action TimeoutElapsed = delegate { };

    private static readonly Timer Timer = new Timer(TimeoutInterval);

    public static void ResetTimer()
    {
        Timer.Stop();
        Timer.Start();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Timer.Elapsed += delegate { TimeoutElapsed(); };

        Timer.Start();
    }
}