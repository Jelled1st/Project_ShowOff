using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateCuttable : MonoBehaviour, IObserver
{
    [SerializeField]
    private CuttableFood _cuttable;

    [SerializeField]
    private List<Vector3> _rotations;

    // Start is called before the first frame update
    private void Start()
    {
        Subscribe(_cuttable);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
        if (observerEvent is CuttableCutEvent)
        {
            var oEvent = observerEvent as CuttableCutEvent;
            if (_rotations.Count >= oEvent.state)
            {
                _cuttable.gameObject.transform.DORotate(_rotations[oEvent.state - 1], 0.3f, RotateMode.LocalAxisAdd);
            }
        }
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }
}