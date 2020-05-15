using System.Collections;
using UnityEngine;

namespace Factory
{
    public abstract class Machine : MonoBehaviour
    {
        [SerializeField] private Collider _inputFunnelTrigger;
        [SerializeField] private Transform _output;
        [SerializeField] private float _outputPushForce;

        [Tooltip("Time before output spits")] [SerializeField]
        private float _delay;

        protected abstract GameObject PreDelayAction(GameObject o);
        protected abstract GameObject PostDelayAction(GameObject o);

        private void Start()
        {
            var collider = _inputFunnelTrigger.gameObject.GetComponent<CollisionCallback>();

            if (collider == null)
                collider = _inputFunnelTrigger.gameObject.AddComponent<CollisionCallback>();

            collider.onTriggerEnter += OnTriggerEnterCallback;
        }

        private void OnTriggerEnterCallback(Collider other)
        {
            StartCoroutine(WaitAndExecute(other.gameObject, _delay));
        }


        private IEnumerator WaitAndExecute(GameObject otherGameObject, float delay)
        {
            var processedObject = PreDelayAction(otherGameObject);

            yield return new WaitForSeconds(delay);

            var spitItem = PostDelayAction(processedObject)?.GetComponent<Rigidbody>();
            spitItem.velocity = Vector3.zero;
            spitItem.angularVelocity = Vector3.zero;
            spitItem.transform.position = _output.position;
            spitItem.AddForce(_outputPushForce * _output.right);
        }
    }
}