﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FarmTool : MonoBehaviour, IControllable
{
    enum Functionalities
    {
        Dig,
        Plant,
        Water,
        Heal,
    }

    [SerializeField] Functionalities _functionality;
    [SerializeField] float _cooldown = 3.0f;
    [SerializeField] float _farmPlotCooldown = 3.0f;
    [SerializeField] ProgressBar _cooldownBar;
    private delegate bool FunctionalityFunctions(FarmPlot plot, float cooldown);
    private FunctionalityFunctions _functionaliesHandler;
    private float _timeSinceLastUse = 0.0f;
    private RawImage _image;
    private bool _isOnCooldown = false;

    void Awake()
    {
        _image = this.GetComponent<RawImage>();
        _cooldownBar.SetActive(false);
    }

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
            case Functionalities.Heal:
                _functionaliesHandler = FarmPlot.Heal;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastUse += Time.deltaTime;
        if(_timeSinceLastUse < _cooldown)
        {
            if(_isOnCooldown) _cooldownBar.SetPercentage(1 - (_timeSinceLastUse / _cooldown));
        }
        else if(_isOnCooldown)
        {
            OnBecomesUseable();
            _isOnCooldown = false;
        }
    }

    private void OnUse()
    {
        _timeSinceLastUse = 0;
        _isOnCooldown = true;
        _image.color = new Color(0.5f, 0.5f, 0.5f);
        _cooldownBar.SetActive(true);
    }

    private void OnBecomesUseable()
    {
        _cooldownBar.SetActive(false);
        _image.color = new Color(1.0f, 1.0f, 1.0f);
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
        if (!_isOnCooldown)
        {
            FarmPlot plot;
            if (hitInfo.gameObject.TryGetComponent<FarmPlot>(out plot))
            {
                if (_functionaliesHandler(plot, _farmPlotCooldown))
                {
                    OnUse();
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

    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<FarmTool>());
        copy.layer = 0;
        copy.transform.SetParent(this.transform.parent.transform.parent);
        return copy;
    }
}
