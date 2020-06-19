using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg_barrels : MonoBehaviour
{
    [SerializeField] private int PlayCount;
    private Rigidbody _rigidBody;
    private int _playCount;

    void Start()
    {
        _playCount = PlayCount;
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if(_playCount > 0)
        {
            _rigidBody.AddRelativeTorque(0, -200, 0);
        }
        if(_playCount == 0)
        {
            _rigidBody.AddRelativeForce(-300, 0, 0);
        }
        _playCount--;
    }
}
