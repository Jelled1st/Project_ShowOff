using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class FinishTrigger : MonoBehaviour
{
    public static event Action<GameObject> FinishTriggerHit;

    private void OnTriggerEnter(Collider other)
    {
        FinishTriggerHit.Invoke(other.gameObject);
    }
}