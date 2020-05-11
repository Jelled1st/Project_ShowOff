using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    //private RaycastHit _lastHit;
    private ControllerHitInfo _hitInfo;

    private Vector3 _lastMousePosition;
    private List<Vector3> _swipePositions;
    private bool _swipeStarted = false;
    private bool _currentlySwiping = false;

    private bool _debugOutput = false;

    //UI raycasting
    [SerializeField] private Canvas _canvas;
    private GraphicRaycaster _graphicRaycaster;
    private EventSystem _eventSystem;


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

        if (_canvas != null)
        {
            if (_canvas.TryGetComponent<GraphicRaycaster>(out _graphicRaycaster))
            {
                _graphicRaycaster = _canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            if (_canvas.TryGetComponent<EventSystem>(out _eventSystem))
            {
                _eventSystem = _canvas.gameObject.AddComponent<EventSystem>();
            }
        }
        else Debug.LogWarning("No canvas added to controller");
    }

    // Update is called once per frame
    void Update()
    {
        bool mousePressed = Input.GetMouseButton(0);
        HandleSwipe(mousePressed);
        if (mousePressed)
        {
            if(_canvas != null) HandleUIRaycast();
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                IControllable controllable;
                if (hit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                {
                    HitControllable(controllable, new ControllerHitInfo(controllable, hit));
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

            if (_timeHeld >= _holdTime)
            {
                OnHoldRelease(_timeHeld, _selected);
            }
            else
            {
                if (!_swipeStarted) OnPress(_selected, _hitPoint, _hitInfo);
            }

            _selected = null;
            _timeHeld = 0;
        }
    }

    private void HandleUIRaycast()
    {
        PointerEventData pointerEventData = new PointerEventData(_eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerEventData, results);
        for (int i = 0; i < results.Count; ++i)
        {
            IControllable controllable;
            if (results[i].gameObject.TryGetComponent<IControllable>(out controllable))
            {
                HitControllable(controllable, new ControllerHitInfo(controllable, results[i]));
            }
        }
    }

    private void HitControllable(IControllable controllable, ControllerHitInfo hitInfo)
    {
        _hitInfo = hitInfo;
        OnClick(controllable, hitInfo.point, hitInfo);
        if (_swipeStarted)
        {
            OnSwipe(GetLastSwipeDirection(), _lastMousePosition, controllable, hitInfo);
        }
        else
        {
            _hitPoint = hitInfo.point;
            //only if not swiping
            if (_selected != null && _selected != controllable)
            {
                if (_timeHeld >= _holdTime)
                {
                    OnHoldRelease(_timeHeld, _selected);
                }
                else
                {
                    if (!_swipeStarted) OnPress(_selected, _hitPoint, hitInfo);
                }

                _timeHeld = 0;
            }
            _selected = controllable;
            _timeHeld += Time.deltaTime;
            if (_timeHeld >= _holdTime)
            {
                OnHold(_timeHeld, _selected, _hitPoint, hitInfo);
            }
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
                OnSwipe(GetLastSwipeDirection(), _lastMousePosition, null, _hitInfo);
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

    public void OnClick(IControllable pressed, Vector3 hitPoint, ControllerHitInfo hitInfo)
    {
        pressed.OnClick(hitPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnClick(hitInfo);
        }
    }

    public void OnPress(IControllable pressed, Vector3 hitPoint, ControllerHitInfo hitInfo)
    {
        pressed.OnPress(hitPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPress(hitInfo);
        }
    }

    public void OnHold(float holdTime, IControllable held, Vector3 hitPoint, ControllerHitInfo hitInfo)
    {
        held.OnHold(holdTime, hitPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnHold(holdTime, hitInfo);
        }
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
        released.OnHoldRelease(timeHeld);
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnHoldRelease(timeHeld, released);
        }
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, IControllable swiped, ControllerHitInfo hitInfo)
    {
        if(swiped != null) swiped.OnSwipe(direction, lastPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnSwipe(direction, lastPoint, hitInfo);
        }
    }
}
