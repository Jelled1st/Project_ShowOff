﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchFeedback : MonoBehaviour, IControlsObserver
{
    [SerializeField] private ParticleSystem _pressFeedback;
    [SerializeField] private ParticleSystem _swipeFeedback;

    private TouchController _touchController;
    private bool _isHeld = false;
    private bool _isSwiping = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject controller = GameObject.FindGameObjectWithTag("Controller");
        if (controller == null) Debug.LogError("No controller found!");
        else
        {
            _touchController = controller.GetComponent<TouchController>();
            Subscribe(_touchController);
        }
    }

    void Update()
    {
        if(_isSwiping == false)
        {
            _swipeFeedback.gameObject.SetActive(false);
        }
        _isSwiping = false;
    }

    public void OnClick(ControllerHitInfo hitInfo)
    {

    }

    public void OnPress(ControllerHitInfo hitInfo)
    {
        if (hitInfo.uiElement) return;
        _pressFeedback.transform.position = hitInfo.point + hitInfo.normal*0.1f;
        _pressFeedback.transform.forward = hitInfo.normal;
        _pressFeedback.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        _pressFeedback.transform.DOComplete();
        _pressFeedback.transform.DOPunchScale(new Vector3(3.0f, 3.0f, 3.0f), 0.3f, 0);
        _pressFeedback.Play();
    }

    public void OnHold(float holdTime, ControllerHitInfo hitInfo)
    {
        if (hitInfo.uiElement) return;
        _pressFeedback.transform.position = hitInfo.point + hitInfo.normal * 0.1f;
        _pressFeedback.transform.forward = hitInfo.normal;
        _pressFeedback.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        if(!_isHeld)
        {
            _pressFeedback.transform.DOComplete();
            _pressFeedback.transform.DOScale(new Vector3(2.0f, 2.0f, 2.0f), 0.2f);
            ParticleSystem.MainModule main = _pressFeedback.main;
            main.loop = true;
            _isHeld = true;
        }
        else if(!DOTween.IsTweening(_pressFeedback.transform))
        {
            _pressFeedback.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            _pressFeedback.transform.DOPunchScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0);
        }

        if(!_pressFeedback.isPlaying) _pressFeedback.Play();
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
        _isHeld = false;
        ParticleSystem.MainModule main = _pressFeedback.main;
        main.loop = false;
        _pressFeedback.transform.DOComplete();
        _pressFeedback.transform.localScale = new Vector3(0, 0, 0);
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo)
    {
        _isSwiping = true;
        _swipeFeedback.gameObject.SetActive(true);
        Vector3 screenPoint = lastPoint + direction;
        screenPoint.z = 1;
        Vector3 mouse3d = Camera.main.ScreenToWorldPoint(screenPoint);

        _swipeFeedback.transform.position = mouse3d;
        if(!_swipeFeedback.isPlaying)_swipeFeedback.Play();
    }
    public void OnDrag(Vector3 position, IControllable dragged)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
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
