using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BKM : MonoBehaviour
{
    //[SerializeField] private int currentMusic;
    [FMODUnity.EventRef, SerializeField] private string background;
    [SerializeField, Range(0.0f, 1.0f)] private float volumeTesting = 0.45f;
    
    private FMOD.Studio.EventInstance _music;

    private void Start()
    {
        _music = FMODUnity.RuntimeManager.CreateInstance(background);
        //_music.setParameterByName("song", currentMusic);
        _music.setVolume(volumeTesting);
        _music.start();
    }
}
