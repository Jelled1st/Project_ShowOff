using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour, IControllable
{
    [Tooltip("Amount of time an object needs to be clicked before onHold() is called, if an object is held less than this time it will be registered as a single press")]
    [SerializeField] private float _holdTime = 0.3f;
    [Tooltip("Minimal amount of speed required for a press and move for it to be registered as a swipe")]
    [SerializeField] private float _swipeSpeed = 1;
    [Tooltip("Minimal distance for a swipe to be registered")]
    [SerializeField] private float _swipeDistance = 5;
    private IControllable _selected = null;
    private float _timeHeld = 0.0f;

    private Vector3 _lastMousePosition;
    private List<Vector3> _swipePositions;
    private bool _currentlySwiping = false;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = "Controller";

        _swipePositions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleSwipe();
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                IControllable controllable;
                if (hit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                {
                    if (_currentlySwiping)
                    {
                        controllable.OnSwipe(GetLastSwipeDirection());
                        OnSwipe(GetLastSwipeDirection());
                    }
                    else
                    {
                        //only if not swiping
                        if (_selected != null && _selected != controllable)
                        {
                            NotifyControls(_selected);
                            _timeHeld = 0;
                        }
                        _selected = controllable;
                        _timeHeld += Time.deltaTime;
                        if (_timeHeld >= _holdTime)
                        {
                            _selected.OnHold(_timeHeld);
                            OnHold(_timeHeld);
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
            NotifyControls(_selected);
            _selected = null;
            _timeHeld = 0;
        }
    }

    private void NotifyControls(IControllable controllable)
    {
        if (_timeHeld >= _holdTime)
        {
            controllable.OnHoldRelease(_timeHeld);
            OnHoldRelease(_timeHeld);
        }
        else
        {
            controllable.OnPress();
            OnPress();
        }
    }

    private void HandleSwipe()
    {
        Vector3 mousePos = Input.mousePosition;
        if ((mousePos - _lastMousePosition).magnitude >= _swipeSpeed)
        {
            _swipePositions.Add(mousePos);
            //register swipe
            if (!_currentlySwiping)
            {
                if (GetFullLengthOfSwipe() > _swipeDistance)
                {
                    _currentlySwiping = true;
                    //Debug.Log("Swiping");
                }
            }
        }
        else
        {
            //if there was a swipe, it ended
            _swipePositions.Clear();
            _currentlySwiping = false;
        }
        _lastMousePosition = mousePos;
    }

    private Vector3 GetLastSwipeDirection()
    {
        int lastIndex = _swipePositions.Count-1;
        return _swipePositions[lastIndex] - _swipePositions[lastIndex - 1];
    }

    private Vector3 GetAvergaSwipeDirection()
    {
        return new Vector3();
    }

    private float GetFullLengthOfSwipe()
    {
        float length = 0;

        for(int i = 0; i < _swipePositions.Count -1; ++i)
        {
            length += (_swipePositions[i + 1] - _swipePositions[i]).magnitude;
        }

        return length;
    }

    public void OnPress()
    {
        //Debug.Log("Press");
    }

    public void OnHold(float holdTime)
    {
        //Debug.Log("Hold " + holdTime);
    }

    public void OnHoldRelease(float timeHeld)
    {
        //Debug.Log("HoldRelease " + timeHeld);
    }

    public void OnSwipe(Vector3 direction)
    {
        //Debug.Log("Swipe " + direction);
    }
}
