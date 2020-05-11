using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : MonoBehaviour, IControllable
{
    enum State
    {
        Withered = -1,

        Rough = 0,
        Dug,
        Planted,
        Grown,
    };

    private State _state;

    // Start is called before the first frame update
    void Start()
    {
        _state = State.Rough;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static bool Dig(FarmPlot plot)
    {
        return plot.Dig();
    }

    public static bool Plant(FarmPlot plot)
    {
        return plot.Plant();
    }

    public static bool Water(FarmPlot plot)
    {
        return plot.Water();
    }

    public bool Dig()
    {
        if (_state == State.Rough)
        {
            Debug.Log("Digging");
            ++_state;
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Plant()
    {
        if (_state == State.Dug)
        {
            Debug.Log("Planting");
            ++_state;
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }

    public bool Water()
    {
        if (_state == State.Planted)
        {
            Debug.Log("Watering");
            ++_state;
            return true;
        }
        else
        {
            Debug.Log("Not allowed");
            return false;
        }
    }


    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnPress(Vector3 hitPoint)
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
}
