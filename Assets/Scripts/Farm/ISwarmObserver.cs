using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwarmObserver : IObserver
{
    void OnBugKill(SwarmUnit unit);
    void OnSwarmDestroy();
    void OnFlee();
    void OnBugSpawn(SwarmUnit unit);
    void OnBugspawnFail();
}
