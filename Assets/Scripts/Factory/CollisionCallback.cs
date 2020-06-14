using System;
using UnityEngine;

namespace Factory
{
    [DisallowMultipleComponent]
    public class CollisionCallback : MonoBehaviour
    {
        public event Action<Collider> TriggerEnter = delegate { };

        private void Awake()
        {
            var collider = GetComponent<Collider>();

            if (!collider.isTrigger)
                collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other);
        }
    }
}