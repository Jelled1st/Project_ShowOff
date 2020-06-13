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
        if(observerEvent is CuttableCutEvent)
        {
            CuttableCutEvent oEvent = observerEvent as CuttableCutEvent;
            if (_moves.Count >= oEvent.state)
            {
                _cuttable.gameObject.transform.DOMove(this.transform.position + _moves[oEvent.state-1], 0.3f);
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
