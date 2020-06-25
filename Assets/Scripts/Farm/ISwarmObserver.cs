public interface ISwarmObserver : IObserver
{
    void OnBugKill(SwarmUnit unit);
    void OnSwarmDestroy();
    void OnFlee();
    void OnBugSpawn(SwarmUnit unit);
    void OnBugspawnFail();
}