using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Factory
{
    public class SubstitutingMachine : Machine
    {
        [BoxGroup("Substitution settings")]
        [SerializeField]
        private GameObject[] _substitute;

        private void Awake()
        {
            if (_substitute.Length == 0)
                Debug.LogError($"The substitute prefab for [{gameObject.name}] is not set!");
        }

        protected override GameObject PreDelayProcess(GameObject inputGameObject)
        {
            Destroy(inputGameObject);
            return Instantiate(_substitute[Random.Range(0, _substitute.Length)]);
        }

        protected override GameObject PostDelayProcess(GameObject outputGameObject)
        {
            return outputGameObject;
        }
    }
}