using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveCuttable : MonoBehaviour, IObserver
{
    [SerializeField] private CuttableFood _cuttable;
    [SerializeField] private List<Vector3> _moves;

    // Start is called before the first frame update
    void Start()
    {
        Subscribe(_cuttable);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnNotify(AObserverEvent observerEvent)
    {
        if(observerEvent is CuttableCut)
        {
            CuttableCut oEvent = observerEvent as CuttableCut;
            if (_moves.Count >= oEvent.state)
            {
                _cuttable.gameObject.transform.DOMove(_moves[oEvent.state-1], 0.3f);
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
