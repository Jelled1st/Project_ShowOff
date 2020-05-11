using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlotFunctionality : MonoBehaviour, IControllable
{
    enum Functionalities
    {
        Plant,
        Water,
    }

    [SerializeField] Functionalities _functionality;
    private delegate void FunctionalityFunctions(FarmPlot plot);
    private FunctionalityFunctions _functionaliesHandler;

    // Start is called before the first frame update
    void Start()
    {
        switch(_functionality)
        {
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
}
