using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [FMODUnity.EventRef]
    private string _sfxEvent;

    private bool _isPlaying = false;

    [SerializeField]
    private float clipLength;

    private FMOD.Studio.EventInstance _swarmInstance;

    [HideInInspector]
    public bool swarmInstancePlaying = false;

    private FMOD.Studio.EventInstance _conveyorHumInstance;

    [HideInInspector]
    public bool conveyorHumInstancePlaying = false;

    /* Wherever the sound needs to play
     [In the area where it needs to play]
     _soundScript.Sound[Name]();     
    */

    #region Event Subscriptions

    private void OnEnable()
    {
        FlatConveyorBelt.ConveyorTurned += SoundRotate;
        FlatConveyorBelt.SpecialConveyorHeld += SoundSpecialConveyor;
        Machine.ItemEnteredMachine += SoundItemEnteredMachine;
        Machine.ItemLeftMachine += SoundItemLeftMachine;
    }

    private void OnDisable()
    {
        FlatConveyorBelt.ConveyorTurned -= SoundRotate;
        FlatConveyorBelt.SpecialConveyorHeld -= SoundSpecialConveyor;
        Machine.ItemEnteredMachine -= SoundItemEnteredMachine;
        Machine.ItemLeftMachine -= SoundItemLeftMachine;
    }

    #endregion

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

    #endregion

    #region FactorySounds

    private void SoundItemLeftMachine(Machine.MachineType machineType)
    {
        // TODO: Add sounds
        switch (machineType)
        {
            case Machine.MachineType.PotatoWasher:

                break;
            case Machine.MachineType.PotatoPeeler:

                break;
            case Machine.MachineType.FryPacker:

                break;
            case Machine.MachineType.FryCutter:

                break;
        }
    }

    private void SoundItemEnteredMachine(Machine.MachineType machineType)
    {
        switch (machineType)
        {
            case Machine.MachineType.PotatoWasher:
                SoundWashing();
                break;
            case Machine.MachineType.PotatoPeeler:
                SoundPeeling();
                break;
            case Machine.MachineType.FryPacker:
                SoundPacking();
                break;
            case Machine.MachineType.FryCutter:
                SoundChopping();
                break;
        }
    }

    private void SoundSpecialConveyor(FlatConveyorBelt.SpecialBeltType specialBeltType, bool isHolding)
    {
        if (isHolding)
        {
            switch (specialBeltType)
            {
                case FlatConveyorBelt.SpecialBeltType.SpeedUp:
                    SoundSpeedup();
                    break;
                case FlatConveyorBelt.SpecialBeltType.SpeedDown:
                    SoundSlowdown();
                    break;
            }
        }
    }

    // Constant humming noise of the conveyors
    public void SoundConveyor()
    {
        if (!conveyorHumInstancePlaying)
        {
            Debug.Log("Conveyor Hum Is Playing");
            _conveyorHumInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX_FACTORY/Conveyor Hum");
            _conveyorHumInstance.start();

            conveyorHumInstancePlaying = true;
        }

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    public void SoundConveyorStop()
    {
        conveyorHumInstancePlaying = false;
        Debug.Log("Stopping Conveyor Hum");
        _conveyorHumInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _conveyorHumInstance.release();
    }

    // Sound when rotating conveyor belts
    public void SoundRotate()
    {
        Play("SFX_FACTORY/Conveyor Rotate");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when chopping potatoes into fries in the machine
    public void SoundChopping()
    {
        Play("SFX_FACTORY/Fry Chopping");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when packing the potatoes in the machine
    public void SoundPacking()
    {
        Play("SFX_FACTORY/Fry Packing");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when machine starts breaking down
    public void SoundBreaking()
    {
        Play("SFX_FACTORY/Machine Breaking");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when a machine fully breaks
    public void SoundBroken()
    {
        Play("SFX_FACTORY/Machine Broken");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when repairing a machine
    public void SoundRepair()
    {
        Play("SFX_FACTORY/Machine Fixing");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when peeling a potato in the machine
    public void SoundPeeling()
    {
        Play("SFX_FACTORY/Potato Peeling");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound when a potato gets washed in the machine
    public void SoundWashing()
    {
        Play("SFX_FACTORY/Potato Washing");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound for slowing down a conveyor belt
    public void SoundSlowdown()
    {
        Play("SFX_FACTORY/Slowdown");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    // Sound Speeding up a conveyor belt
    public void SoundSpeedup()
    {
        Play("SFX_FACTORY/Speedup");

        clipLength = 0.1f;
        StartCoroutine(WaitForEnd(clipLength));
    }

    #endregion

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
        if (length <= 0)
            throw new ArgumentOutOfRangeException("Length of clip is not set in " + " " + gameObject.name +
                                                  nameof(length));

        length = clipLength;
        yield return new WaitForSeconds(length);
        _isPlaying = false;
    }

    #endregion
}