using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    [RequireComponent(typeof(Collider))]
    [SelectionBase]
    public abstract class Machine : MonoBehaviour, IControllable
    {
        [BoxGroup("Processing settings")]
        [SerializeField]
        [Required]
        private Collider _inputFunnelTrigger;

        [BoxGroup("Processing settings")]
        [SerializeField]
        [Required]
        private Transform _output;

        [BoxGroup("Processing settings")]
        [SerializeField]
        private float _outputPushForce;

        [BoxGroup("Processing settings")]
        [SerializeField]
        private ParticleSystem _particleSystem;

        [BoxGroup("Processing settings")]
        [Tooltip("Time before output spits")]
        [SerializeField]
        private float _delay;

        [BoxGroup("Clogging settings")]
        [SerializeField]
        private float _baseFixTime = 1f;

        [BoxGroup("Clogging settings")]
        [SerializeField]
        private ParticleSystem _clogVisual;

        [BoxGroup("Clogging settings")]
        [SerializeField]
        private GameObject _repairVisuals;

        [BoxGroup("Clogging settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _slowPerStage = 1f / _stagesToBreak;

        [BoxGroup("Clogging settings")]
        [MinMaxSlider(1f, 10f)]
        [SerializeField]
        private Vector2 _breakEverySeconds = new Vector2(4, 8);

        private const int _stagesToBreak = 3;

        private int _currentClogStage;
        private bool _isRepairing;

        private float Delay { get; set; }

        private bool IsClogged => _currentClogStage == _stagesToBreak;

        // If we later need it - it's the filtration by tag
        // [SerializeField] private string _allowedInputTag;
        // [SerializeField] private string _outputTag;
        // protected string AllowedInputTag => _allowedInputTag;
        // protected string OutputTag => _outputTag;

        protected abstract GameObject PreDelayProcess(GameObject o);
        protected abstract GameObject PostDelayProcess(GameObject o);

        private void Start()
        {
            // Initial reset
            Delay = _delay;
            _currentClogStage = 0;
            _isRepairing = false;

            // Set input trigger callback
            if (!TryGetComponent(out CollisionCallback collisionCallback))
            {
                Debug.LogWarning(
                    $"[{gameObject.name}] lacks {nameof(CollisionCallback)} script! Edit the prefab! Trying to add it...");
            }

            collisionCallback = _inputFunnelTrigger.gameObject.AddComponent<CollisionCallback>();
            collisionCallback.onTriggerEnter += OnTriggerEnterCallback;

            _particleSystem = _particleSystem.NullIfEqualsNull();
            _particleSystem?.Stop();

            _repairVisuals = _repairVisuals.NullIfEqualsNull();
            _repairVisuals?.SetActive(false);

            WaitAndClog();
        }

        private void Clog()
        {
            if (_isRepairing || IsClogged)
                return;

            _currentClogStage++;

            if (IsClogged)
            {
                Scores.AddScore(Scores.MachineCompleteBreakage);
            }

            // Increase delay by slowPerStage %
            Delay *= 1 + _slowPerStage;

            _clogVisual.Play(true);

            // Debug.Log($"Clog [{_currentClogStage}] {gameObject.name}");

            WaitAndClog();
        }

        private void Repair()
        {
            if (_currentClogStage <= 0 || _isRepairing)
                return;

            _repairVisuals?.SetActive(true);
            _isRepairing = true;

            // Debug.Log($"Started repairing {gameObject.name}");
            DOTween.Sequence().AppendInterval(_baseFixTime * _currentClogStage)
                .AppendCallback(() =>
                {
                    // Debug.Log($"Finished repairing {gameObject.name}");
                    _currentClogStage--;
                    _repairVisuals?.SetActive(false);
                    _isRepairing = false;

                    // Decrease delay by slowPerStage %
                    Delay /= 1 + _slowPerStage;

                    WaitAndClog();
                });
        }

        private void WaitAndClog()
        {
            DOTween.Sequence().AppendInterval(Random.Range(_breakEverySeconds.x, _breakEverySeconds.y))
                .AppendCallback(Clog);
        }

        private void OnTriggerEnterCallback(Collider other)
        {
            StartCoroutine(WaitAndExecute(other.gameObject, Delay));
        }


        private IEnumerator WaitAndExecute(GameObject otherGameObject, float delay)
        {
            if (IsClogged)
                yield break;

            var check = otherGameObject.layer == LayerMask.NameToLayer("ConveyorBelt");
            if (check)
                yield break;

            var processedObject = PreDelayProcess(otherGameObject);

            _particleSystem?.Play();
            yield return new WaitForSeconds(delay);
            _particleSystem?.Stop();

            // Reset and spit the rigidbody
            var spitItem = PostDelayProcess(processedObject)?.GetComponent<Rigidbody>();
            spitItem.velocity = Vector3.zero;
            spitItem.angularVelocity = Vector3.zero;
            spitItem.transform.position = _output.position;
            spitItem.AddForce(_outputPushForce * _output.right);
        }


        public void OnPress(Vector3 hitPoint)
        {
            Repair();
        }

        public void OnClick(Vector3 hitPoint)
        {
        }

        public void OnHold(float holdTime, Vector3 hitPoint)
        {
        }

        public void OnHoldRelease(float timeHeld)
        {
        }

        public void OnSwipe(Vector3 direction, Vector3 lastPoint)
        {
        }

        public void OnDrag(Vector3 position)
        {
        }

        public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
        {
        }

        public void OnDragDropFailed(Vector3 position)
        {
        }

        public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
        {
        }

        public GameObject GetDragCopy()
        {
            return null;
        }
    }
}