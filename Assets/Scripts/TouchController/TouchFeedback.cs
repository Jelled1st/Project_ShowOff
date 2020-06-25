using UnityEngine;
using DG.Tweening;

public class TouchFeedback : MonoBehaviour, IControlsObserver
{
    [SerializeField]
    private ParticleSystem _pressFeedback;

    [SerializeField]
    private ParticleSystem _swipeFeedback;

    [SerializeField]
    private bool _doPressFeedback = true;

    [SerializeField]
    private bool _doHoldFeedback = true;

    private TouchController _touchController;
    private bool _isHeld = false;
    private bool _isSwiping = false;

    private GameObject _dragCopy = null;

    private void Start()
    {
        var controller = GameObject.FindGameObjectWithTag("Controller");
        if (controller == null)
        {
            Debug.LogError("No controller found!");
        }
        else
        {
            _touchController = controller.GetComponent<TouchController>();
            Subscribe(_touchController);
        }
    }

    private void Update()
    {
        if (_isSwiping == false && _swipeFeedback != null && !_swipeFeedback.Equals(null))
        {
            _swipeFeedback.gameObject.SetActive(false);
        }

        _isSwiping = false;
    }

    public GameObject GetCurrentDragCopy()
    {
        return _dragCopy;
    }

    public void OnClick(ControllerHitInfo hitInfo)
    {
    }

    public void OnPress(ControllerHitInfo hitInfo)
    {
        if (hitInfo.uiElement || !_doPressFeedback) return;
        _pressFeedback.transform.position = hitInfo.point + hitInfo.normal * 0.1f;
        _pressFeedback.transform.forward = hitInfo.normal;
        _pressFeedback.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        _pressFeedback.transform.DOComplete();
        _pressFeedback.transform.DOPunchScale(new Vector3(3.0f, 3.0f, 3.0f), 0.3f, 0);
        _pressFeedback.Play();
    }

    public void OnHold(float holdTime, ControllerHitInfo hitInfo)
    {
        if (hitInfo.uiElement || !_doHoldFeedback) return;
        _pressFeedback.transform.position = hitInfo.point + hitInfo.normal * 0.1f;
        _pressFeedback.transform.forward = hitInfo.normal;
        _pressFeedback.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        if (!_isHeld)
        {
            _pressFeedback.transform.DOComplete();
            _pressFeedback.transform.DOScale(new Vector3(2.0f, 2.0f, 2.0f), 0.2f);
            var main = _pressFeedback.main;
            main.loop = true;
            _isHeld = true;
        }
        else if (!DOTween.IsTweening(_pressFeedback.transform))
        {
            _pressFeedback.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            _pressFeedback.transform.DOPunchScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0);
        }

        if (!_pressFeedback.isPlaying) _pressFeedback.Play();
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
        _isHeld = false;
        var main = _pressFeedback.main;
        main.loop = false;
        _pressFeedback.transform.DOComplete();
        _pressFeedback.transform.localScale = new Vector3(0, 0, 0);
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo)
    {
        var screenPoint = lastPoint + direction;
        //screenPoint.z = 1;
        //Vector3 mouse3d = Camera.main.ScreenToWorldPoint(screenPoint);

        if (_dragCopy == null) DoSwipeParticle(screenPoint);
    }

    private void DoSwipeParticle(Vector3 point)
    {
        _swipeFeedback.gameObject.SetActive(true);
        _isSwiping = true;

        var toCamera = Camera.main.transform.position - point;
        point = Camera.main.transform.position - toCamera.normalized;

        _swipeFeedback.transform.position = point;
        if (!_swipeFeedback.isPlaying) _swipeFeedback.Play();
    }

    public void OnDrag(Vector3 position, IControllable dragged, ControllerHitInfo hitInfo)
    {
        if (_dragCopy == null)
        {
            _dragCopy = dragged.GetDragCopy();
            if (_dragCopy != null && !hitInfo.uiElement)
            {
                var originalPos = position;
                var toCamera = Camera.main.transform.position - position;
                position = Camera.main.transform.position - toCamera.normalized;

                _dragCopy.transform.position = position;
                _dragCopy.transform.localScale /= (Camera.main.transform.position - originalPos).magnitude;
            }
        }

        if (_dragCopy != null) // else would not call this if _dragCopy had just been set
        {
            if (hitInfo.uiElement)
            {
                _dragCopy.transform.position = Input.mousePosition;
            }
            else
            {
                var originalPos = position;
                var toCamera = Camera.main.transform.position - position;
                position = Camera.main.transform.position - toCamera.normalized;

                _dragCopy.transform.position = position;
            }
        }
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        if (_dragCopy != null) Destroy(_dragCopy);
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
    {
        if (_dragCopy != null) Destroy(_dragCopy);
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }
}