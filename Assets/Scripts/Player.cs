using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log("hit da " + hit.transform.gameObject.name);
                GameObject hitObject = hit.transform.gameObject;
                ConveyorBelt conveyor = null;
                if (hitObject.TryGetComponent<ConveyorBelt>(out conveyor))
                {
                    conveyor.Turn();
                    return;
                }
                ConveyorPusherBlock conveyorPusherBlock;
                if (hitObject.TryGetComponent<ConveyorPusherBlock>(out conveyorPusherBlock))
                {
                    conveyor = conveyorPusherBlock.GetConveyorBelt();
                    conveyor.Turn();
                    return;
                }
                FlatConveyorBelt flatConveyorBelt;
                if (hitObject.TryGetComponent<FlatConveyorBelt>(out flatConveyorBelt))
                {
                    flatConveyorBelt.Turn();
                    return;
                }
                FlatConveyorBeltCurve flatConveyorBeltCurve;
                if (hitObject.TryGetComponent<FlatConveyorBeltCurve>(out flatConveyorBeltCurve))
                {
                    flatConveyorBeltCurve.Turn();
                    return;
                }
            }
        }
    }
}
