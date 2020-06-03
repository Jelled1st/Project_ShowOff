using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class BKM : MonoBehaviour
{
    [FMODUnity.EventRef, SerializeField] private string musicFarm = "event:/BKM/Farm Music";
    [FMODUnity.EventRef, SerializeField] private string farmTruckDrive = "event:/SFX_TRUCK/Farm Truck Driving";
    [FMODUnity.EventRef, SerializeField] private string farmTruckTransit = "event:/SFX_TRUCK/Farm Truck Transit";
    
    [FMODUnity.EventRef, SerializeField] private string musicFactory = "event:/BKM/Factory Music";
    private FMOD.Studio.EventInstance _instanceSong;
    private FMOD.Studio.EventInstance _instanceTransitionFx;

    public int debugLevelTest;
    private void Start()
    {
        if (debugLevelTest == 0)
            FarmMusic();
        
        if (debugLevelTest == 1)
            FactoryMusic();
    }

    
    //Debugging if functions work properly
    private void Update()
    {
    /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TruckDriving();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            TruckTransition();
        }
        */
    }

    private void FarmMusic() 
    { 
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(musicFarm);
        _instanceSong.start();
    }
    public void TruckDriving()
    {
        _instanceTransitionFx = FMODUnity.RuntimeManager.CreateInstance(farmTruckDrive);
        _instanceTransitionFx.start();
        StartCoroutine(WaitForEnd(6.0f));
    }

    public void TruckTransition()
    {
        _instanceTransitionFx = FMODUnity.RuntimeManager.CreateInstance(farmTruckTransit);
        _instanceTransitionFx.start();
        _instanceTransitionFx.release();
    }

    private void FactoryMusic()
    {
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(musicFactory);
        _instanceSong.start();
    }
    private IEnumerator WaitForEnd(float length)
    {
        yield return new WaitForSeconds(length);
        _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _instanceTransitionFx.release();
        _instanceSong.release();
    }
}
