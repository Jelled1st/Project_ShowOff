using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour, ISubject, IGameHandlerObserver
{
    [Tooltip("Amount of time an object needs to be clicked before onHold() is called, if an object is held less than this time it will be registered as a single press")]
    [SerializeField] private float _holdTime = 0.3f;

    List<IControlsObserver> _observers = new List<IControlsObserver>();

    // General
    private IControllable _selected = null;
    private GameObject _selectedGameObject = null;
    private IControllable _previousSelected = null;
    private bool _paused = false;
    
    // Holding
    private float _timeHeld = 0.0f;
    private ControllerHitInfo _hitInfo;

    // Swiping
    private Vector3 _lastMousePosition;
    private List<Vector3> _swipePositions;
    private bool _swipeStarted = false;
    private bool _currentlySwiping = false;
    [Tooltip("Minimal amount of speed required for a press and move for it to be registered as a swipe, swipeSpeed will be scaled by 0.005 because of measuring in 3D distance")]
    [SerializeField] private float _swipeSpeed = 1;
    [Tooltip("Minimal distance for a swipe to be registered as swipe")]
    [SerializeField] private float _swipeDistance = 1;

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
    private bool _debugLog = false;

    void Awake()
    {
        this.gameObject.tag = "Controller";
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler != null && gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        _swipePositions = new List<Vector3>();
        _swipeSpeed = Mathf.Abs(_swipeSpeed) * 0.005f;
        _swipeDistance = Mathf.Abs(_swipeDistance);

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
        //if (_paused) return;
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
                _selected = controllable;
            }
            else //if UI wasn't hit, try on world objects
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if(_debugLog) Debug.Log("Hitcast hit: " + hit.transform.gameObject);
                    if (hit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                    {
                        hitInfo = new ControllerHitInfo(controllable, hit);
                        HitControllable(controllable, new ControllerHitInfo(controllable, hit));
                    }
                    else
                    {
                        //ResetPressAndHold();
                        HitNonControllable(new ControllerHitInfo(null, hit));
                    }
                }
                else ResetPressAndHold();
            }
        }
        else
        {
            ResetPressAndHold();
        }
        HandleDragCalls();
        HandleSwipe(mousePressed, controllable != null);
    }

    private void ResetPressAndHold()
    {
        if (_selectedGameObject != null)
        {
            if(Input.GetMouseButton(0) && _selected != null) //mouse is still down
            {
                if (!_isDragging)
                {
                    StartDrag();
                }
            }

            if (_timeHeld >= _holdTime)
            {
                OnHoldRelease(_timeHeld, _selected);
            }
            else
            {
                if (!_swipeStarted && !_isDragging) OnPress(_selected, _hitInfo);
            }
            
            _selected = null;
            _selectedGameObject = null;
            _timeHeld = 0;
        }
        _wasDraggingLastFrame = _isDragging;
        if(!Input.GetMouseButton(0)) _isDragging = false;
    }

    private void StartDrag()
    {
        _dragSelected = _selected;
        _dragStartInfo = _hitInfo;
        _isDragging = true;
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
        if (_swipeStarted)
        {
            OnSwipe(GetLastSwipeDirection(), _lastMousePosition, controllable, hitInfo);
        }
        else if(!_isDragging)// Not dragging nor swiping
        {
            //only if not swiping
            if (_selected != null && _selected != controllable)
            {
                StartDrag();
                if (_timeHeld >= _holdTime)
                {
                    OnHoldRelease(_timeHeld, _selected);
                }
                else
                {
                    if (!_swipeStarted && !_isDragging) OnPress(_selected, hitInfo);
                }

                _timeHeld = 0;
            }
            _selected = controllable;
            _selectedGameObject = hitInfo.gameObject;
            _timeHeld += Time.deltaTime;
            if (_timeHeld >= _holdTime)
            {
                OnHold(_timeHeld, _selected, hitInfo);
            }
        }
    }

    private void HitNonControllable(ControllerHitInfo hitInfo)
    {
        OnClick(null, hitInfo);

        if (!_isDragging && !_swipeStarted) // Not dragging nor swiping
        {
            if (_selected != null)
            {
                StartDrag();
            }
            else _selected = null;

            //if (_selected != null)
            //{
            //    StartDrag();
            //    if (_timeHeld >= _holdTime)
            //    {
            //        OnHoldRelease(_timeHeld, _selected);
            //    }
            //    else
            //    {
            //        if (!_swipeStarted && !_isDragging) OnPress(_selected, hitInfo);
            //    }

            //    _timeHeld = 0;
            //}

            _hitInfo = hitInfo;
            _selectedGameObject = hitInfo.gameObject;
            _timeHeld += Time.deltaTime;
            if (_timeHeld >= _holdTime)
            {
                OnHold(_timeHeld, _selected, hitInfo);
            }
        }
    }

    // Paramaters are not the start values, but values for if something new was hit this frame
    private void HandleDragCalls()
    {
        if (_dragStartInfo.gameObject == null)
        {
            _isDragging = false;
            _wasDraggingLastFrame = false;
        }
        Vector3 mouse3d = new Vector3();
        if (_isDragging || _wasDraggingLastFrame) mouse3d = Get3dCursorPosition((_dragStartInfo.gameObject.transform.position - Camera.main.transform.position).magnitude);
        else return;
        if (_isDragging)
        {
            OnDrag(mouse3d, _dragSelected, _dragStartInfo);
        }
        else
        {
            IControllable controllable = null;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                {
                }
            }

            if (_wasDraggingLastFrame && controllable != null)
            {
                OnDragDrop(mouse3d, _dragSelected, controllable, new ControllerHitInfo(controllable, hit));
            }
            else if (_wasDraggingLastFrame && controllable == null)
            {
                OnDragDropFailed(mouse3d, _dragSelected);
            }
        }
    }

    private void HandleSwipe(bool mousePressed, bool hitControllable)
    {
        Vector3 mousePos = Get3dCursorPosition(0.02f);
        if (mousePressed)
        {
            if ( _currentlySwiping || ( (Mathf.Abs((mousePos - _lastMousePosition).magnitude) >= _swipeSpeed || (_swipeStarted)) ) ) //if a controllable was hit swiped are not allowed to start
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
                        if (_debugLog) Debug.Log("Started swiping");
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

    public void OnNotify(AObserverEvent observerEvent)
    {

    }

    public void OnClick(IControllable pressed, ControllerHitInfo hitInfo)
    {
        pressed?.OnClick(hitInfo.point);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnClick(hitInfo);
        }
    }

    public void OnPress(IControllable pressed, ControllerHitInfo hitInfo)
    {
        pressed?.OnPress(hitInfo.point);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnPress(hitInfo);
        }
    }

    public void OnHold(float holdTime, IControllable held, ControllerHitInfo hitInfo)
    {
        held?.OnHold(holdTime, hitInfo.point);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnHold(holdTime, hitInfo);
        }
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
        released?.OnHoldRelease(timeHeld);
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnHoldRelease(timeHeld, released);
        }
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, IControllable swiped, ControllerHitInfo hitInfo)
    {
        swiped?.OnSwipe(direction, lastPoint);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnSwipe(direction, lastPoint, hitInfo);
        }
    }

    public void OnDrag(Vector3 position, IControllable dragged, ControllerHitInfo hitInfo)
    {
        dragged.OnDrag(position);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnDrag(position, dragged, hitInfo);
        }
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        dragged.OnDragDrop(position, droppedOn, hitInfo);
        droppedOn.OnDrop(dragged, hitInfo);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnDragDrop(position, dragged, droppedOn, hitInfo);
        }
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
    {
        dragged.OnDragDropFailed(position);
        for (int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnDragDropFailed(position, dragged);
        }
    }

    public void OnPause()
    {
        _paused = true;
    }

    public void OnContinue()
    {
        _paused = false;
    }

    public void OnFinish()
    {
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    public void Notify(AObserverEvent observerEvent)
    {
    }
}
