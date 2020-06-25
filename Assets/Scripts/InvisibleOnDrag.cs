using UnityEngine;

public class InvisibleOnDrag : MonoBehaviour, IControlsObserver
{
    [SerializeField]
    private GameObject _controllable;

    [SerializeField]
    private TouchController _controller;

    [SerializeField]
    private TouchFeedback _controllerFeedback;

    private IControllable _controllableScriptRef;
    private bool _enabledDragCopy = false;
    private bool _dragging = false;

    public bool active = true;

    private void Start()
    {
        if (_controllable == null) _controllable = gameObject;
        _controllableScriptRef = _controllable.GetComponent<IControllable>();
        Subscribe(_controller);
    }

    private void Update()
    {
        if (!active) return;
        if (_dragging && !_enabledDragCopy)
        {
            _enabledDragCopy = TryEnableDragCopyRenderer();
        }
    }

    private bool TryEnableDragCopyRenderer()
    {
        var dragCopy = _controllerFeedback.GetCurrentDragCopy();
        if (dragCopy != null)
        {
            var renderers = dragCopy.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }

            return true;
        }

        return false;
    }

    public void OnClick(ControllerHitInfo hitInfo)
    {
    }

    public void OnDrag(Vector3 position, IControllable dragged, ControllerHitInfo hitInfo)
    {
        if (!active) return;
        if (!_dragging && _controllableScriptRef == dragged)
        {
            var renderers = _controllable.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = false;
            }

            _enabledDragCopy = TryEnableDragCopyRenderer();
            _dragging = true;
        }
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        if (!active) return;
        if (dragged == _controllableScriptRef)
        {
            var renderers = _controllable.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }

            _enabledDragCopy = false;
            _dragging = false;
        }
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
    {
        if (!active) return;
        if (dragged == _controllableScriptRef)
        {
            var renderers = _controllable.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }

            _enabledDragCopy = false;
            _dragging = false;
        }
    }

    public void OnHold(float holdTime, ControllerHitInfo hitInfo)
    {
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }

    public void OnPress(ControllerHitInfo hitInfo)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo)
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
}