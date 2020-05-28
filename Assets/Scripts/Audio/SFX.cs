using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [FMODUnity.EventRef] private string _sfxEvent;
    private bool _isPlaying = false;
    [SerializeField] private float clipLength;
    private FMOD.Studio.EventInstance _swarmInstance;

    [HideInInspector] public bool swarmInstancePlaying = false;
    
    /* Wherever the sound needs to play
     [In the area where it needs to play]
     _soundScript.Sound[Name]();     
    */

    #region FarmSounds
    // Digging Shovel Sound
    public void SoundDig()
    {
        Play("SFX_FARM/Dig");
        
        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Watering can Sound
    public void SoundWater()
    {
        Play("SFX_FARM/Water");
        
        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    // Plant growing through stages Sound
    public void SoundPlantGrowth()
    {
        Play("SFX_FARM/Grow");
        
        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    // Sound Spraying vitamins
    public void SoundPesticide()
    {
        Play("SFX_FARM/Pesticide");
        
        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    // Sound before dragging into the truck
    public void SoundUproot()
    {
        Play("SFX_FARM/Uproot");
        
        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    #endregion
    
    // Sound Locust Swarm
    public void SoundSwarm()
    {
        if (!swarmInstancePlaying)
        {
            Debug.Log("Spawning Swarm Sound");
            _swarmInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_FARM/Swarm");
            _swarmInstance.start();

            swarmInstancePlaying = true;
        }

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound Locust Kill
    public void SoundSwarmStop()
    {
        if (swarmInstancePlaying)
        {
            swarmInstancePlaying = false;
            Debug.Log("Stopping swarm instance audio");
            _swarmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _swarmInstance.release();   
        }
    }
    
    #region playRules
    public void Play(string fmodEvent)
    {
        Debug.Log("Audio is playing: " + fmodEvent);
        if (_isPlaying == false)
        {
            _sfxEvent = "event:/" + fmodEvent;
                
            FMODUnity.RuntimeManager.PlayOneShot(_sfxEvent, transform.position);
                
            _isPlaying = true;
        }
    }

    private IEnumerator WaitForEnd(float length)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException("Length of clip is not set in " + " " + gameObject.name + nameof(length));
            
        length = clipLength;
        yield return new WaitForSeconds(length);
        _isPlaying = false;
    }
    #endregion
}
