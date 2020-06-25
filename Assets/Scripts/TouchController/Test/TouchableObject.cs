using UnityEngine;
using UnityEngine.Events;

public class TouchableObject : MonoBehaviour, IControllable
{
    [SerializeField]
    private UnityEvent _onPress;

    private bool _wasSwiped = false;
    private bool _swiping = false;

    private GameObject clone;

    private void Update()
    {
        if (_swiping == false)
        {
            _wasSwiped = false;
        }

        _swiping = false;
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
        var color = GetRandomColor();
        GetComponent<Renderer>().material.color = color;
        _onPress.Invoke();
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
        transform.localScale = new Vector3(1 + holdTime / 3, 1 + holdTime / 3, 1 + holdTime / 3);
    }

    public void OnHoldRelease(float timeHeld)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
        _swiping = true;
        if (!_wasSwiped)
        {
            //_wasSwiped = true;
            transform.Rotate(Vector3.up, -direction.x / 7, Space.World);
            transform.Rotate(Vector3.right, direction.y / 7, Space.World);
        }
    }

    private Color GetRandomColor()
    {
        return new Color((float) Random.Range(0f, 1f), (float) Random.Range(0f, 1f), (float) Random.Range(0f, 1f));
    }

    public void OnDrag(Vector3 position)
    {
        if (clone == null)
        {
            clone = Instantiate(gameObject);
            Destroy(clone.GetComponent<TouchableObject>());
            Destroy(clone.GetComponent<BoxCollider>());
            clone.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.3f);
        }

        clone.transform.position = position;
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        Destroy(clone);
        clone = null;
    }

    public void OnDragDropFailed(Vector3 position)
    {
        Destroy(clone);
        clone = null;
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        var copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<TouchableObject>());
        Destroy(copy.GetComponent<BoxCollider>());
        return copy;
    }
}