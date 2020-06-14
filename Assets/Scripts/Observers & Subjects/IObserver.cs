using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void Subscribe(ISubject subject);
    void UnSubscribe(ISubject subject);
    void OnNotify(AObserverEvent observerEvent);
}
