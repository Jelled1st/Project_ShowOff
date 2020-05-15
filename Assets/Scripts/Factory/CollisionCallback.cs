using System;
using UnityEngine;

namespace Factory
{
    public class CollisionCallback : MonoBehaviour
    {
        public event Action<Collider> onTriggerEnter = delegate { };

        private void Awake()
        {
            var collider = GetComponent<Collider>();
            
            if (!collider.isTrigger)
                collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }
    }
}