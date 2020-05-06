using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class FinishTrigger : MonoBehaviour
{
    [Tooltip("Invoked when a rigidbody entered the attached trigger")] [SerializeField]
    private FinishTriggerHit _finishTriggerHit;

    private Collider _finishTrigger;
    public FinishTriggerHit FinishTriggerHit => _finishTriggerHit;

    private void Awake()
    {
        _finishTriggerHit = new FinishTriggerHit();
    }

    private void OnTriggerEnter(Collider other)
    {
        FinishTriggerHit.Invoke(other.gameObject);
    }
}

[Serializable]
public class FinishTriggerHit : UnityEvent<GameObject>
{
}