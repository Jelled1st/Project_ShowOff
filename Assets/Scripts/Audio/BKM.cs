using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BKM : MonoBehaviour
{
    [FMODUnity.EventRef, SerializeField] private string _musicFarm = "event:/BKM/Farm Music";
    [FMODUnity.EventRef, SerializeField] private string _truckDrive = "event:/SFX_TRUCK/Truck Driving";
    [FMODUnity.EventRef, SerializeField] private string _truckIdle = "event:/SFX_TRUCK/Truck Idle";
    //[FMODUnity.EventRef] private string _musicFactory;

    private FMOD.Studio.EventInstance _instanceSong;
    private FMOD.Studio.EventInstance _instanceTransitionFx;
    
    private void Start()
    {
        FarmMusic();
    }

    /*
     Debugging if functions work properly
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TruckDriving();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            TruckTransition();
        }
    }
    */
    
    private void FarmMusic()
    {
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(_musicFarm);
        _instanceSong.start();
    }
    public void TruckDriving()
    {
        _instanceTransitionFx = FMODUnity.RuntimeManager.CreateInstance(_truckDrive);
        _instanceTransitionFx.start();
        StartCoroutine(WaitForEnd(13.42f));
    }

    public void TruckTransition()
    {
        _instanceTransitionFx = FMODUnity.RuntimeManager.CreateInstance(_truckIdle);
        _instanceTransitionFx.start();
        _instanceTransitionFx.release();
    }

    private IEnumerator WaitForEnd(float length)
    {
        yield return new WaitForSeconds(length);
        _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _instanceTransitionFx.release();
        _instanceSong.release();
    }
}
