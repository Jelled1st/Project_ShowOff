using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Factory
{
    public class SubstitutingMachine : Machine
    {
        [SerializeField] private GameObject[] _substitute;

        private void Awake()
        {
            if (_substitute.Length == 0)
                Debug.LogError($"The substitute prefab for [{gameObject.name}] is not set!");
        }

        protected override GameObject PreDelayAction(GameObject o)
        {
            Destroy(o);
            return null;
        }

        protected override GameObject PostDelayAction(GameObject o)
        {
            return Instantiate(_substitute[Random.Range(0, _substitute.Length)]);
        }
    }
}