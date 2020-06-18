public interface IObserver
{
    void Subscribe(ISubject subject);
    void UnSubscribe(ISubject subject);
    void OnNotify(AObserverEvent observerEvent);
}
