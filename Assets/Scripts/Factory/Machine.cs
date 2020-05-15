using System.Collections;
using UnityEngine;

namespace Factory
{
    public abstract class Machine : MonoBehaviour
    {
        [SerializeField] private Collider _inputFunnelTrigger;
        [SerializeField] private Transform _output;
        [SerializeField] private float _outputPushForce;
        [SerializeField] private ParticleSystem _particleSystem;

        [Tooltip("Time before output spits")] [SerializeField]
        private float _delay;

        // If we later need it - it's the filtration by tag
        // [SerializeField] private string _allowedInputTag;
        // [SerializeField] private string _outputTag;
        // protected string AllowedInputTag => _allowedInputTag;
        // protected string OutputTag => _outputTag;

        protected abstract GameObject PreDelayAction(GameObject o);
        protected abstract GameObject PostDelayAction(GameObject o);

        private void Start()
        {
            var collider = _inputFunnelTrigger.gameObject.GetComponent<CollisionCallback>();

            if (collider == null)
                collider = _inputFunnelTrigger.gameObject.AddComponent<CollisionCallback>();

            collider.onTriggerEnter += OnTriggerEnterCallback;
            
            if (_particleSystem.Equals(null))
                _particleSystem = null;
            
            _particleSystem?.Stop();
        }

        private void OnTriggerEnterCallback(Collider other)
        {
            StartCoroutine(WaitAndExecute(other.gameObject, _delay));
        }


        private IEnumerator WaitAndExecute(GameObject otherGameObject, float delay)
        {
            var processedObject = PreDelayAction(otherGameObject);

            _particleSystem?.Play();
            yield return new WaitForSeconds(delay);
            _particleSystem?.Stop();

            // Reset and spit the rigidbody
            var spitItem = PostDelayAction(processedObject)?.GetComponent<Rigidbody>();
            spitItem.velocity = Vector3.zero;
            spitItem.angularVelocity = Vector3.zero;
            spitItem.transform.position = _output.position;
            spitItem.AddForce(_outputPushForce * _output.right);
        }
    }
}