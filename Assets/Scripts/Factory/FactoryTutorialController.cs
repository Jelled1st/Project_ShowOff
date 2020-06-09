using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Factory;
using NaughtyAttributes;
using StateMachine;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class TutorialPart
{
    public CollisionCallback _trigger;

    public GameObject _visuals;
    // public Transform _cameraPosition;

    [NonSerialized]
    private bool _passed;

    [NonSerialized]
    private bool _awaitingPass;

    public void Init()
    {
        if (_trigger != null && !_trigger.Equals(null))
        {
            _trigger.TriggerEnter += OnTriggerEnter;
        }

        _visuals.SetActive(false);
    }

    public void Cleanup()
    {
        if (_trigger != null && !_trigger.Equals(null))
            _trigger.TriggerEnter -= OnTriggerEnter;

        DOTween.Sequence()
            // .Append(Camera.main.transform.DOMove(FactoryTutorialController.CameraInitialPosition, 2f))
            .AppendCallback(() =>
            {
                Time.timeScale = 1f;
                _visuals.SetActive(false);
            }).SetUpdate(true);
    }

    private void OnTriggerEnter(Collider collider)
    {
        ForceInvokeTrigger();
    }

    public void ForceInvokeTrigger()
    {
        if (_passed)
            return;

        Time.timeScale = FactoryTutorialController.TimeScaleChange;
        // Object.FindObjectsOfType<FlatConveyorBelt>().ToggleAll(false);

        Vector3 initialScale = _visuals.transform.localScale;

        DOTween.Sequence()
            // .Append(Camera.main.transform.DOMove(_cameraPosition.position, 2f))
            .AppendCallback(() => { _visuals.SetActive(true); })
            .AppendCallback(() => CoroutineHandler.StartUniqueCoroutine(WaitForPassed()))
            // .AppendInterval(3f).AppendCallback(() =>
            // {
            //     Debug.Log("passed interval");
            //     _passed = true;
            // })
            .SetUpdate(true);
    }

    private IEnumerator WaitForPassed()
    {
        _awaitingPass = true;
        yield return new WaitUntil(() => _passed);
        Cleanup();
    }

    public void Passed()
    {
        if (_awaitingPass)
            _passed = true;
    }
}

public class CoroutineHandler : LazySingleton<CoroutineHandler>
{
    public static void StartUniqueCoroutine(IEnumerator coroutine)
    {
        Instance.StartCoroutine(coroutine);
    }
}

public class FactoryTutorialController : MonoBehaviour
{
    [BoxGroup("Slowdown")]
    [SerializeField]
    private TutorialPart _slowdownPart;

    [BoxGroup("Slowdown")]
    [SerializeField]
    private FlatConveyorBelt _slowdownBelt;


    [BoxGroup("Speedup")]
    [SerializeField]
    private TutorialPart _speedupPart;


    [BoxGroup("Speedup")]
    [SerializeField]
    private FlatConveyorBelt _speedupBelt;


    [BoxGroup("Rotation")]
    [SerializeField]
    private TutorialPart _rotationPart;


    [BoxGroup("Rotation")]
    [SerializeField]
    private FlatConveyorBelt _rotationBelt;

    [BoxGroup("Machine breaking")]
    [Label("Ignore trigger")]
    [SerializeField]
    private TutorialPart _machineBreakPart;


    [BoxGroup("Basic")]
    [SerializeField]
    private float _timeToHold = 1.5f;

    [BoxGroup("Basic")]
    [SerializeField]
    private float _timeScaleChange = 0.5f;

    public static float TimeScaleChange => FindObjectOfType<FactoryTutorialController>()._timeScaleChange;

    private void OnEnable()
    {
        _slowdownPart.Init();
        _speedupPart.Init();
        _rotationPart.Init();
        _machineBreakPart.Init();

        _slowdownBelt.BeltHeld += delegate(float f)
        {
            if (f > _timeToHold) _slowdownPart?.Passed();
        };
        _speedupBelt.BeltHeld += delegate(float f)
        {
            if (f > _timeToHold) _speedupPart?.Passed();
        };
        _rotationBelt.BeltPressed += delegate { _rotationPart?.Passed(); };

        Machine.MachineBreaking += OnMachineBreak;
        Machine.MachineStartedRepairing += OnMachineRepair;
    }

    private void OnMachineRepair()
    {
        _machineBreakPart.Passed();
        Machine.MachineStartedRepairing -= OnMachineRepair;
    }

    private void OnMachineBreak(Machine machine)
    {
        var visualPosition = Camera.main.WorldToScreenPoint(machine.transform.position);

        _machineBreakPart._visuals.transform.position = visualPosition;
        _machineBreakPart.ForceInvokeTrigger();

        Machine.MachineBreaking -= OnMachineBreak;
    }
}