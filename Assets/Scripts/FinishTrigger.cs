using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishTrigger : MonoBehaviour
{
    public event Action<GameObject> FinishTriggerHit = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        FinishTriggerHit.Invoke(other.gameObject);
    }
}