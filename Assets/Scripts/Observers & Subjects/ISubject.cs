using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubject
{
    void Register(IObserver observer);

    void UnRegister(IObserver observer);

    void OnNotify(AObserverEvent observerEvent);
}
