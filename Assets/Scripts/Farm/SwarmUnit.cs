using UnityEngine;

public class SwarmUnit : MonoBehaviour, IControllable
{
    private Swarm _swarm;

    [SerializeField]
    private Vector3 _hoverMovementScale = new Vector3(0, 0.2f, 0.1f);

    [SerializeField]
    private Vector3 _sinTimeScale = new Vector3(1, 5.0f, 0.3f);

    public void Init(Swarm swarm)
    {
        _swarm = swarm;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var hoverMove = new Vector3(
            Mathf.Sin(Time.realtimeSinceStartup * _sinTimeScale.x) * _hoverMovementScale.x,
            Mathf.Sin(Time.realtimeSinceStartup * _sinTimeScale.y) * _hoverMovementScale.y,
            Mathf.Sin(Time.realtimeSinceStartup * _sinTimeScale.z) * _hoverMovementScale.z);

        gameObject.transform.localPosition += hoverMove * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        FarmPlot plot;
        if (other.gameObject.TryGetComponent<FarmPlot>(out plot))
        {
            _swarm.UnitReachedPlot(this, plot);
        }
    }

    #region IControllable

    public void OnClick(Vector3 hitPoint)
    {
        _swarm.UnitHit(this);
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        return null;
    }

    #endregion
}