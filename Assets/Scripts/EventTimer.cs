using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EventTimer : MonoBehaviour
{
    [SerializeField]
    private float _time;

    [SerializeField]
    private UnityEvent _timerEndEvent;

    [SerializeField]
    private bool _pauseTimeOnEnd;

    [SerializeField]
    private bool _useUnscaledDeltaTime;

    [SerializeField]
    private bool _countDown = false;

    [SerializeField]
    private TextMeshProUGUI _output;

    private float _count;
    private bool _paused = false;
    private bool _reachedTime = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (_pauseTimeOnEnd || _countDown) _timerEndEvent.AddListener(Pause);
    }

    // Update is called once per frame
    private void Update()
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
        if (_countDown) return Mathf.Max(_time - _count, 0);
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