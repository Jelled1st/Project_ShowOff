using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EventTimer : MonoBehaviour
{
    [SerializeField] float _time;
    [SerializeField] UnityEvent _timerEndEvent;
    [SerializeField] bool _pauseTimeOnEnd;
    [SerializeField] bool _useUnscaledDeltaTime;
    [SerializeField] bool _countDown = false;
    [SerializeField] TextMeshProUGUI _output;
    float _count;
    bool _paused = false;
    bool _reachedTime = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_pauseTimeOnEnd || _countDown) _timerEndEvent.AddListener(Pause);
    }

    // Update is called once per frame
    void Update()
    {
        if (_paused) return;
        if (_useUnscaledDeltaTime) _count += Time.unscaledDeltaTime;
        else _count += Time.deltaTime;

        if (_count >= _time && !_reachedTime)
        {
            _reachedTime = true;
            _timerEndEvent.Invoke();
        }
        _output.text = "" + Mathf.Floor(GetTime());
    }

    public float GetTime()
    {
        if (_countDown) return Mathf.Max((_time - _count), 0);
        else return _count;
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
