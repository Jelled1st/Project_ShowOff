using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveCuttable : MonoBehaviour, IObserver
{
    [SerializeField]
    private CuttableFood _cuttable;

    [SerializeField]
    private List<Vector3> _moves;

    private void Start()
    {
        Subscribe(_cuttable);
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
        if (observerEvent is CuttableCutEvent)
        {
            var oEvent = observerEvent as CuttableCutEvent;
            if (_moves.Count >= oEvent.state)
            {
                _cuttable.gameObject.transform.DOMove(transform.position + _moves[oEvent.state - 1], 0.3f);
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