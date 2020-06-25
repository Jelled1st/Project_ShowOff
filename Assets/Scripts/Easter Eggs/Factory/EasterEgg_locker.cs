using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg_locker : MonoBehaviour
{
    private bool _isClicked = false;

    private Rigidbody _rigidBody;

    private void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (!_isClicked)
        {
            _rigidBody.AddRelativeForce(0, 0, 750f);
        }

        _isClicked = true;
    }
}