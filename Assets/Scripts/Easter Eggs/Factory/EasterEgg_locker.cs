using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg_locker : MonoBehaviour
{
    private bool _isClicked = false;
    private Rigidbody _rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (!_isClicked)
        {
            _rigidBody.AddRelativeForce(0, 0, 60f);
        }
        _isClicked = true;
    }
}
