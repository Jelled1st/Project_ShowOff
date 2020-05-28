using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Truck : MonoBehaviour, IGameHandlerObserver
{
    [SerializeField] private float _acceleration = 1.0f;
    private Rigidbody _rigidbody;
    private bool _drive = false;
    
    [SerializeField] private BKM musicManager;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
    }

    void FixedUpdate()
    {
        if(_drive)
        {
            _rigidbody.velocity += this.transform.right * _acceleration * Time.deltaTime;
        }
    }

    public void OnPause()
    {
    }

    public void OnContinue()
    {
    }

    public void OnFinish()
    {
        musicManager.TruckDriving();
        _drive = true;
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.Register(this);
    }
}
