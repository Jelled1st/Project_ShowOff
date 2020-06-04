using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFactoryController : MonoBehaviour
{
    
    public GameObject finishTriggerLevel1;
    public GameObject finishTriggerLevel2;
    public GameObject spawner;
    public GameObject level1Machines;
    public GameObject level2Machines;
    public FlatConveyorBelt[] bufferBelts;
    public FlatConveyorBelt[] level1Belts;
    public Transform level2CameraPosition;

    private void Start()
    {
        level2Machines.SetActive(false);
        foreach (var flatConveyorBelt in bufferBelts)
        {
            flatConveyorBelt.Speed = 0;
        }
        
    }
    
    
}
