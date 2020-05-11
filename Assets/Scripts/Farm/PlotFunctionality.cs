using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlotFunctionality : MonoBehaviour, IControllable
{
    enum Functionalities
    {
        Dig,
        Plant,
        Water,
    }

    [SerializeField] Functionalities _functionality;
    private delegate bool FunctionalityFunctions(FarmPlot plot);
    private FunctionalityFunctions _functionaliesHandler;

    // Start is called before the first frame update
    void Start()
    {
        switch(_functionality)
        {
            case Functionalities.Dig:
                _functionaliesHandler = FarmPlot.Dig;
                break;
            case Functionalities.Plant:
                _functionaliesHandler = FarmPlot.Plant;
                break;
            case Functionalities.Water:
                _functionaliesHandler = FarmPlot.Water;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(Vector3 hitPoint)
    {
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
        FarmPlot plot;
        if (hitInfo.gameObject.TryGetComponent<FarmPlot>(out plot))
        {
            _functionaliesHandler(plot);
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }
}
