using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmGameHandler : MonoBehaviour, IControlsObserver
{
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

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick(RaycastHit hit)
    {
    }

    public void OnPress(RaycastHit hit)
    {
        FarmPlot plot;
        if (hit.transform.gameObject.TryGetComponent<FarmPlot>(out plot))
        {
            Debug.Log("Hit plot " + plot);
        }
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
