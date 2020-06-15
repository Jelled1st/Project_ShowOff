using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTimer : MonoBehaviour
{
    [SerializeField] float _time;
    [SerializeField] UnityEvent _timerEndEvent;
    [SerializeField] bool _pauseTimeOnEnd;
    [SerializeField] bool _useUnscaledDeltaTime;
    float _count;
    bool _paused = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_pauseTimeOnEnd) _timerEndEvent.AddListener(Pause);
    }

    // Update is called once per frame
    void Update()
    {
        if (_paused) return;
        if (_useUnscaledDeltaTime) _count += Time.unscaledDeltaTime;
        else _count += Time.deltaTime;

        if (_count >= _time) _timerEndEvent.Invoke();
    }

    public void ResetTimer()
    {
        _time = 0;
    }

    public void Pause()
    {
        _paused = true;
    }

    public void Unpause()
    {
        _paused = false;
    }
}
