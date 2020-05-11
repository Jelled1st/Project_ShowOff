using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchableObject : MonoBehaviour, IControllable
{
    bool _wasSwiped = false;
    bool _swiping = false;

    GameObject clone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(_swiping == false)
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
        Color color = GetRandomColor();
        this.GetComponent<Renderer>().material.color = color;
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
        this.transform.localScale = new Vector3(1 + holdTime/3, 1 + holdTime/3, 1 + holdTime/3);
    }

    public void OnHoldRelease(float timeHeld)
    {
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
        _swiping = true;
        if(!_wasSwiped)
        {
            //_wasSwiped = true;
            this.transform.Rotate(Vector3.up, -direction.x/7, Space.World);
            this.transform.Rotate(Vector3.right, direction.y/7, Space.World);
        }
    }

    private Color GetRandomColor()
    {
        return new Color((float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f));
    }

    public void OnDrag(Vector3 position)
    {
        if (clone == null)
        {
            clone = Instantiate(this.gameObject);
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
}
