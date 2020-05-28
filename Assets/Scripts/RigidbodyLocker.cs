using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyLocker : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.TryGetComponent(out Rigidbody rigidbody);
        if (!rigidbody.Equals(null))
        {
            rigidbody.freezeRotation = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        other.gameObject.TryGetComponent(out Rigidbody rigidbody);
        if (!rigidbody.Equals(null))
        {
            rigidbody.freezeRotation = false;
        }
    }
}