using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    
    [FMODUnity.EventRef] public string sfxEvent;
    //public string sfxEvent;
        
    private bool _isPlaying = false;

    [SerializeField] private float clipLength;

    public void SoundDig()
    {
        Play("SFX_FARM/Dig");
        
        clipLength = 0.9f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    public void SoundWater()
    {
        Play("SFX_FARM/Water");
        
        clipLength = 2.9f;
        StartCoroutine(WaitForEnd(clipLength));
    }
    
    /* Wherever the sound needs to play
     [In the script]
     private GameObject _audioManager;
     
     Start() 
     {
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager");
     }
     
     [In the area where it needs to play]
     _audioManager.GetComponent<SFX>().SoundPlaceholder();     
     */

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
