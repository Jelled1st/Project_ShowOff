using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchFeedback : MonoBehaviour, IControlsObserver
{
    [SerializeField] private ParticleSystem _pressFeedback;

    private TouchController _touchController;

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


    public void OnPress(RaycastHit hit)
    {
        _pressFeedback.transform.position = hit.point + hit.normal*0.1f;
        _pressFeedback.transform.forward = hit.normal;
        _pressFeedback.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        _pressFeedback.transform.DOComplete();
        _pressFeedback.transform.DOPunchScale(new Vector3(3.0f, 3.0f, 3.0f), 0.3f, 0);
        _pressFeedback.Play();
    }

    public void OnHold(float holdTime, RaycastHit hit)
    {
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, RaycastHit hit)
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
