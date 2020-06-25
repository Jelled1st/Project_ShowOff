using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FarmTool : MonoBehaviour, IControllable, ISubject
{
    private enum Functionalities
    {
        Dig,
        Plant,
        Water,
        Heal
    }

    [SerializeField]
    private Functionalities _functionality;

    [SerializeField]
    private float _cooldown = 3.0f;

    [SerializeField]
    private float _farmPlotCooldown = 3.0f;

    [SerializeField]
    private ProgressBar _cooldownBar;

    [SerializeField]
    private Canvas _parentCanvas;

    [SerializeField]
    private Vector3 _dragScale = new Vector3(1.0f, 1.0f, 1.0f);

    private delegate bool FunctionalityFunctions(FarmPlot plot, float cooldown, FarmTool tool);

    private FunctionalityFunctions _functionaliesHandler;
    private float _timeSinceLastUse = 0.0f;
    private RawImage _image;
    private bool _isOnCooldown = false;

    private List<IObserver> _observers = new List<IObserver>();

    private void Awake()
    {
        _image = GetComponent<RawImage>();
        _cooldownBar.SetActive(false);
        gameObject.tag = "FarmTool";
    }

    private void Start()
    {
        switch (_functionality)
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

    private void Update()
    {
        _timeSinceLastUse += Time.deltaTime;
        if (_timeSinceLastUse < _cooldown)
        {
            if (_isOnCooldown) _cooldownBar.SetPercentage(1 - _timeSinceLastUse / _cooldown);
        }
        else if (_isOnCooldown)
        {
            OnBecomesUseable();
            _isOnCooldown = false;
        }
    }

    private void Wiggle()
    {
        transform.DOPunchRotation(new Vector3(0, 0, 20), 0.3f, 50);
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
                if (_functionaliesHandler(plot, _farmPlotCooldown, this))
                {
                    OnUse();
                }
                else
                {
                    Wiggle();
                }
            }
        }
        else
        {
            Notify(new ToolOnCooldownWarningEvent(this));
            Wiggle();
            Debug.Log("Use is on cooldown");
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        var copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<FarmTool>());
        copy.transform.localScale = _dragScale;
        copy.layer = 0;
        copy.transform.SetParent(_parentCanvas.transform);
        return copy;
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}