using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SFX : MonoBehaviour
{
    
    [FMODUnity.EventRef, HideInInspector] public string sfxEvent;
    //public string sfxEvent;
        
    private bool _isPlaying = false;

    [SerializeField] private float clipLength;

    /* Wherever the sound needs to play
     [In the script]
     private GameObject _audioManager;
     private SFX _soundScript
     
     Start() 
     {
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        _soundScript = _audioManager.GetComponent<SFX>().
     }
     
     [In the area where it needs to play]
     _soundScript.Sound[Name]();     
    */
    
    // Digging Shovel Sound

    private FMOD.Studio.EventInstance _swarmInstance;

    #region FarmSounds
    public void SoundDig()
    {
        Play("SFX_FARM/Dig");
        
        clipLength = 0.9f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Watering can Sound
    public void SoundWater()
    {
        Play("SFX_FARM/Water");
        
        clipLength = 2.9f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    // Plant growing through stages Sound
    public void SoundPlantGrowth()
    {
        Play("SFX_FARM/Grow");
        
        clipLength = 0.45f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    // Sound Spraying vitamins
    public void SoundPesticide()
    {
        Play("SFX_FARM/Pesticide");
        
        clipLength = 1.4f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    // Sound before dragging into the truck
    public void SoundUproot()
    {
        Play("SFX_FARM/Uproot");
        
        clipLength = 0.45f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    #endregion
    
    // Sound Locust Swarm
    public void SoundSwarm()
    {
        _swarmInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_FARM/Swarm");
        _swarmInstance.start();

        clipLength = 1.0f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound Locust Kill
    public void SoundSwarmStop()
    {
        Debug.Log("Audio is playing: " + _swarmInstance);
        _swarmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _swarmInstance.release();
    }
    
    #region playRules
    public void Play(string fmodEvent)
    {
        Debug.Log("Audio is playing: " + fmodEvent);
        if (_isPlaying == false)
        {
            sfxEvent = "event:/" + fmodEvent;
                
            FMODUnity.RuntimeManager.PlayOneShot(sfxEvent, transform.position);
                
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
