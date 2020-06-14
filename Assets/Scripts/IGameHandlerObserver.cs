using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameHandlerObserver : IObserver
{
    // OnPause is called when the game gets paused
    void OnPause();
    //OnContinue is called when the game was paused and now continues
    void OnContinue();

    //OnFinish is called when the game is finished (Scene might still be loaded but clear condition has been reached)
    void OnFinish();
}
