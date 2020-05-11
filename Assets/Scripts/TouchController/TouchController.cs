using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour, ISubject
{
    [Tooltip("Amount of time an object needs to be clicked before onHold() is called, if an object is held less than this time it will be registered as a single press")]
    [SerializeField] private float _holdTime = 0.3f;

    List<IControlsObserver> _observers = new List<IControlsObserver>();

    // General
    private IControllable _selected = null;
    
    // Holding
    private float _timeHeld = 0.0f;
    private ControllerHitInfo _hitInfo;

    // Swiping
    private Vector3 _lastMousePosition;
    private List<Vector3> _swipePositions;
    private bool _swipeStarted = false;
    private bool _currentlySwiping = false;
    [Tooltip("Minimal amount of speed required for a press and move for it to be registered as a swipe")]
    [SerializeField] private float _swipeSpeed = 1;
    [Tooltip("Minimal distance for a swipe to be registered as swipe")]
    [SerializeField] private float _swipeDistance = 5;

    // Drag and drop - can be the same as swiping, however a swipe needs a min speed and does not have to start on an object
    private IControllable _dragSelected = null;
    private ControllerHitInfo _dragStartInfo;
    private bool _isDragging = false;
    private bool _wasDraggingLastFrame = false;

    // UI raycasting
    [SerializeField] private Canvas _canvas;
    private GraphicRaycaster _graphicRaycaster;
    private EventSystem _eventSystem;

    // DEBUG
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
        IControllable controllable = null;
        ControllerHitInfo hitInfo = new ControllerHitInfo(true);

        if (mousePressed)
        {
            bool hitUI = false;
            if(_canvas != null) hitUI = HandleUIRaycast(out hitInfo);
            if(hitUI)
            {
                controllable = hitInfo.gameObject.GetComponent<IControllable>();
            }
            else //if UI wasn't hit, try on world objects
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                    {
                        hitInfo = new ControllerHitInfo(controllable, hit);
                        HitControllable(controllable, new ControllerHitInfo(controllable, hit));
                    }
                    else
                    {
                        ResetPressAndHold();
                    }
                }
                else ResetPressAndHold();
            }
        }
        else
        {
            ResetPressAndHold();
        }
        //Debug.Log(controllable != null);
        HandleDragCalls(controllable != null, hitInfo, controllable);
        HandleSwipe(mousePressed, controllable != null);
    }

    private void ResetPressAndHold()
    {
        if (_selected != null)
        {
            if(Input.GetMouseButton(0)) //mouse is still down
            {
                if (!_isDragging)
                {
                    _dragSelected = _selected;
                    _dragStartInfo = _hitInfo;
                    _isDragging = true;
                }
            }

            if (_timeHeld >= _holdTime)
            {
                OnHoldRelease(_timeHeld, _selected);
            }
            else
            {
                if (!_swipeStarted) OnPress(_selected, _hitInfo);
            }

            _selected = null;
            _timeHeld = 0;
        }
        _wasDraggingLastFrame = _isDragging;
        if(!Input.GetMouseButton(0)) _isDragging = false;
    }

    private bool HandleUIRaycast(out ControllerHitInfo hitInfo)
    {
        PointerEventData pointerEventData = new PointerEventData(_eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerEventData, results);

        RaycastResult result;
        IControllable controllable = null;
        for (int i = 0; i < results.Count; ++i)
        {
            if (results[i].gameObject.TryGetComponent<IControllable>(out controllable))
            {
                HitControllable(controllable, new ControllerHitInfo(controllable, results[i]));
                result = results[i];
                hitInfo = new ControllerHitInfo(controllable, result);
                return true;
            }
        }
        //don't reset press, maybe world objects are hit
        hitInfo = new ControllerHitInfo(true);
        return false;
    }

    private void HitControllable(IControllable controllable, ControllerHitInfo hitInfo)
    {
        _hitInfo = hitInfo;
        OnClick(controllable, hitInfo);
        if(_isDragging)
        {
        }
        else if (_swipeStarted)
        {
            OnSwipe(GetLastSwipeDirection(), _lastMousePosition, controllable, hitInfo);
        }
        else // Not dragging nor swiping
        {
            //only if not swiping
            if (_selected != null && _selected != controllable)
            {
                if (_timeHeld >= _holdTime)
                {
                    OnHoldRelease(_timeHeld, _selected);
                }
                else
                {
                    if (!_swipeStarted) OnPress(_selected, hitInfo);
                }

                _timeHeld = 0;
            }
            _selected = controllable;
            _timeHeld += Time.deltaTime;
            if (_timeHeld >= _holdTime)
            {
                OnHold(_timeHeld, _selected, hitInfo);
            }
        }
    }

    // Paramaters are not the start values, but values for if something new was hit this frame
    private void HandleDragCalls(bool hitControllable, ControllerHitInfo hitInfo, IControllable droppedOn)
    {
        Vector3 mouse3d = new Vector3();
        if (_isDragging || _wasDraggingLastFrame) mouse3d = Get3dCursorPosition((_dragStartInfo.gameObject.transform.position - Camera.main.transform.position).magnitude);
        else return;
        if (_isDragging)
        {
            OnDrag(mouse3d, _dragSelected);
        }
        else if(_wasDraggingLastFrame && hitControllable)
        {
            OnDragDrop(mouse3d, _dragSelected, droppedOn, hitInfo);
        }
        else if(_wasDraggingLastFrame && !hitControllable)
        {
            OnDragDropFailed(mouse3d, _dragSelected);
        }
    }

    private void HandleSwipe(bool mousePressed, bool hitControllable)
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePressed)
        {
            if (_currentlySwiping || (Mathf.Abs((mousePos - _lastMousePosition).magnitude) >= _swipeSpeed && !hitControllable && !_isDragging)) //if a controllable was hit swiped are not allowed to start
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

    private Vector3 Get3dCursorPosition(float z)
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = z;
        return Camera.main.ScreenToWorldPoint(screenPoint);
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

    public void OnClick(IControllable pressed, ControllerHitInfo hitInfo)
    {
        pressed.OnClick(hitInfo.point);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnClick(hitInfo);
        }
    }

    public void OnPress(IControllable pressed, ControllerHitInfo hitInfo)
    {
        pressed.OnPress(hitInfo.point);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPress(hitInfo);
        }
    }

    public void OnHold(float holdTime, IControllable held, ControllerHitInfo hitInfo)
    {
        held.OnHold(holdTime, hitInfo.point);
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

    public void OnDrag(Vector3 position, IControllable dragged)
    {
        dragged.OnDrag(position);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnDrag(position, dragged);
        }
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        Debug.Log("Drag dropped");
        dragged.OnDragDrop(position, droppedOn, hitInfo);
        droppedOn.OnDrop(dragged, hitInfo);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnDragDrop(position, dragged, droppedOn, hitInfo);
        }
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
    {
        Debug.Log("Drag dropped failed");
        dragged.OnDragDropFailed(position);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnDragDropFailed(position, dragged);
        }
    }
}
