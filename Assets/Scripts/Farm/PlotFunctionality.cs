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
    [SerializeField] float _cooldown = 3.0f;
    private delegate bool FunctionalityFunctions(FarmPlot plot);
    private FunctionalityFunctions _functionaliesHandler;
    float _timeSinceLastUse = 0.0f;
    bool _freeUseForStart = true;

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
        _timeSinceLastUse += Time.deltaTime;
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
        if (_timeSinceLastUse >= _cooldown || _freeUseForStart)
        {
            FarmPlot plot;
            if (hitInfo.gameObject.TryGetComponent<FarmPlot>(out plot))
            {
                if (_functionaliesHandler(plot))
                {
                    _timeSinceLastUse = 0;
                    _freeUseForStart = false;
                }
            }
        }
        else Debug.Log("Use is on cooldown");
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }
}
