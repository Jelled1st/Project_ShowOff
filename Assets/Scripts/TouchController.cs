using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour, ISubject
{
    List<IControllable> _controllables;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = "Controller";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log("hit da " + hit.transform.gameObject.name);
                IControllable controllable;
                if (hit.transform.gameObject.TryGetComponent<IControllable>(out controllable))
                {
                    controllable.OnPress();
                }
            }
        }
    }

    public void Register(IObserver observer)
    {
        if(observer is IControllable)
        {
            IControllable controllable = (IControllable)observer;
            _controllables.Add(controllable);
        }
    }

    public void UnRegister(IObserver observer)
    {
        if (observer is IControllable)
        {
            IControllable controllable = (IControllable)observer;
            _controllables.Remove(controllable);
        }
    }
}
