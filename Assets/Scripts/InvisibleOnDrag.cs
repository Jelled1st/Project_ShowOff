using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleOnDrag : MonoBehaviour, IControlsObserver
{
    [SerializeField] GameObject _controllable;
    [SerializeField] TouchController _controller;
    [SerializeField] TouchFeedback _controllerFeedback;

    private IControllable _controllableScriptRef;
    private bool _enabledDragCopy = false;
    private bool _dragging = false;

    public bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        _controllableScriptRef = _controllable.GetComponent<IControllable>();
        Subscribe(_controller);
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        if (_dragging && !_enabledDragCopy)
        {
            _enabledDragCopy = TryEnableDragCopyRenderer();
        }
    }

    private bool TryEnableDragCopyRenderer()
    {
        GameObject dragCopy = _controllerFeedback.GetCurrentDragCopy();
        if(dragCopy != null)
        {
            Renderer[] renderers = dragCopy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
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
            Renderer[] renderers = _controllable.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
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
            Renderer[] renderers = _controllable.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
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
            Renderer[] renderers = _controllable.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
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
