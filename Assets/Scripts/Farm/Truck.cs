using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Truck : MonoBehaviour, IGameHandlerObserver
{
    [SerializeField]
    private float _acceleration = 0.4f;

    [SerializeField]
    private float _maxSpeed = 10.0f;

    [SerializeField]
    private float _startupTime = 1.3f;

    private Rigidbody _rigidbody;
    private bool _drive = false;
    private float _timeSinceDrive = 0.0f;

    [SerializeField]
    private BKM musicManager;

    // Start is called before the first frame update
    private void Start()
    {
        var gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (_drive)
        {
            _timeSinceDrive += Time.deltaTime;
            if (_timeSinceDrive > _startupTime)
            {
                _rigidbody.velocity += transform.right * _acceleration;
                _rigidbody.velocity =
                    _rigidbody.velocity.normalized * Mathf.Min(_rigidbody.velocity.magnitude, _maxSpeed);
            }
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

    public void OnNotify(AObserverEvent observerEvent)
    {
    }
}