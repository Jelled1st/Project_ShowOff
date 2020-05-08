using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour, ISubject
{
    [Tooltip("Amount of time an object needs to be clicked before onHold() is called, if an object is held less than this time it will be registered as a single press")]
    [SerializeField] private float _holdTime = 0.3f;
    [Tooltip("Minimal amount of speed required for a press and move for it to be registered as a swipe")]
    [SerializeField] private float _swipeSpeed = 1;
    [Tooltip("Minimal distance for a swipe to be registered as swipe")]
    [SerializeField] private float _swipeDistance = 5;

    List<IControlsObserver> _observers = new List<IControlsObserver>();

    private IControllable _selected = null;
    private float _timeHeld = 0.0f;
    private Vector3 _hitPoint = new Vector3();
    private RaycastHit _lastHit;

    private Vector3 _lastMousePosition;
    private List<Vector3> _swipePositions;
    private bool _swipeStarted = false;
    private bool _currentlySwiping = false;

    private bool _debugOutput = false;


    void Awake()
    {
        this.gameObject.tag = "Controller";
    }

    // Start is called before the first frame update
    void Start()
    {
        _swipePositions = new List<Vector3>();
        _swipeSpeed = Mathf.Abs(_swipeSpeed);
        _swipeDistance = Mathf.Abs(_swipeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        bool mousePressed = Input.GetMouseButton(0);
        HandleSwipe(mousePressed);
        if (mousePressed)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _lastHit))
            {
                IControllable controllable;
                if (_lastHit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                {
                    OnClick(controllable, _lastHit.point, _lastHit);
                    if (_swipeStarted)
                    {
                        OnSwipe(GetLastSwipeDirection(), _lastMousePosition, controllable, _lastHit);
                    }
                    else
                    {
                        _hitPoint = _lastHit.point;
                        //only if not swiping
                        if (_selected != null && _selected != controllable)
                        {
                            NotifyControls(_selected, _lastHit);
                            _timeHeld = 0;
                        }
                        _selected = controllable;
                        _timeHeld += Time.deltaTime;
                        if (_timeHeld >= _holdTime)
                        {
                            OnHold(_timeHeld, _selected, _hitPoint, _lastHit);
                        }
                    }
                }
                else
                {
                    ResetPressAndHold();
                }
            }
            else ResetPressAndHold();
        }
        else
        {
            ResetPressAndHold();
        }
    }

    private void ResetPressAndHold()
    {
        if (_selected != null)
        {
            NotifyControls(_selected, _lastHit);
            _selected = null;
            _timeHeld = 0;
        }
    }

    private void NotifyControls(IControllable controllable, RaycastHit hit)
    {
        if (_timeHeld >= _holdTime)
        {
            OnHoldRelease(_timeHeld, _selected, hit);
        }
        else
        {
            if(_swipeStarted) OnPress(_selected, _hitPoint, hit);
        }
    }

    private void HandleSwipe(bool mousePressed)
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePressed)
        {
            if (_currentlySwiping || Mathf.Abs((mousePos - _lastMousePosition).magnitude) >= _swipeSpeed)
            {
                _swipeStarted = true;
                _swipePositions.Add(mousePos);
                if(_swipePositions.Count == 50)
                {
                    _swipePositions.RemoveAt(0);
                }
                OnSwipe(GetLastSwipeDirection(), _lastMousePosition, null, _lastHit);
                //register swipe
                if (!_currentlySwiping)
                {
                    if (GetFullLengthOfSwipe() > _swipeDistance)
                    {
                        _currentlySwiping = true;
                        if (_debugOutput) Debug.Log("Started swiping");
                    }
                }
            }
            else
            {
                _swipeStarted = false;
                //if there was a swipe, it ended
                _swipePositions.Clear();
                _currentlySwiping = false;
            }
        }
        else
        {
            _swipeStarted = false;
            //if there was a swipe, it ended
            _swipePositions.Clear();
            _currentlySwiping = false;
        }
        _lastMousePosition = mousePos;
    }

    private Vector3 GetLastSwipeDirection()
    {
        if (_swipePositions.Count <= 0) return new Vector3();
        else if (_swipePositions.Count == 1)
        {
            return _swipePositions[0] - _lastMousePosition;
        }
        else
        {
            int lastIndex = _swipePositions.Count - 1;
            return _swipePositions[lastIndex] - _swipePositions[lastIndex - 1];
        }
    }

    private Vector3 GetAvergaSwipeDirection()
    {
        return new Vector3();
    }

    private float GetFullLengthOfSwipe()
    {
        float length = 0;

        for(int i = 0; i < _swipePositions.Count - 1; ++i)
        {
            length += (_swipePositions[i + 1] - _swipePositions[i]).magnitude;
        }

        return length;
    }


    public void Register(IObserver observer)
    {
        if (observer is IControlsObserver)
        {
            _observers.Add((IControlsObserver)observer);
        }
    }

    public void UnRegister(IObserver observer)
    {
        if (observer is IControlsObserver) _observers.Remove((IControlsObserver)observer);
    }

    public void OnClick(IControllable pressed, Vector3 hitPoint, RaycastHit hit)
    {
        pressed.OnClick(hitPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnClick(hit);
        }
    }

    public void OnPress(IControllable pressed, Vector3 hitPoint, RaycastHit hit)
    {
        pressed.OnPress(hitPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPress(hit);
        }
    }

    public void OnHold(float holdTime, IControllable held, Vector3 hitPoint, RaycastHit hit)
    {
        held.OnHold(holdTime, hitPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnHold(holdTime, hit);
        }
    }

    public void OnHoldRelease(float timeHeld, IControllable released, RaycastHit hit)
    {
        released.OnHoldRelease(timeHeld);
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnHoldRelease(timeHeld, released);
        }
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, IControllable swiped, RaycastHit hit)
    {
        if(swiped != null) swiped.OnSwipe(direction, lastPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnSwipe(direction, lastPoint, hit);
        }
    }
}
