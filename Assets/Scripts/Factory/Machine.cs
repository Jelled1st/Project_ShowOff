using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    [SelectionBase]
    public abstract class Machine : MonoBehaviour, IControllable
    {
        [Header("Processing settings")] [SerializeField]
        private Collider _inputFunnelTrigger;

        [SerializeField] private Transform _output;
        [SerializeField] private float _outputPushForce;
        [SerializeField] private ParticleSystem _particleSystem;

        [Tooltip("Time before output spits")] [SerializeField]
        private float _delay;

        [Header("Clogging settings")] [SerializeField]
        private float _baseFixTime = 1f;

        [SerializeField] private ParticleSystem _clogVisual;

        [SerializeField] private GameObject _repairVisuals;


        [Range(0f, 1f)] [SerializeField] private float _slowPerStage = 1 / 3f;

        private const int _stagesToBreak = 3;

        [MinMaxSlider(1f, 10f)] [SerializeField]
        private Vector2 _breakEverySeconds = new Vector2(4, 8);

        private float _delayPrivate;
        private int _currentClogStage;
        private bool _isRepairing;

        private float Delay
        {
            get { return _delayPrivate; }
            set { _delayPrivate = value; }
        }

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
            _delayPrivate = _delay;
            _currentClogStage = 0;
            _isRepairing = false;

            // Set input trigger callback
            var collider = _inputFunnelTrigger.gameObject.GetComponent<CollisionCallback>();

            if (collider == null)
                collider = _inputFunnelTrigger.gameObject.AddComponent<CollisionCallback>();

            collider.onTriggerEnter += OnTriggerEnterCallback;

            // Trick to allow usage of ? for particleSystem
            if (_particleSystem.Equals(null))
                _particleSystem = null;

            _particleSystem?.Stop();

            if (_repairVisuals.Equals(null))
                _repairVisuals = null;

            _repairVisuals?.SetActive(false);

            WaitAndClog();
        }

        private void Clog()
        {
            if (_isRepairing)
                return;

            _currentClogStage++;

            // Just restart timer if already at the last stage
            if (_currentClogStage > _stagesToBreak)
            {
                _currentClogStage = _stagesToBreak;
            }
            else
            {
                // Increase delay by slowPerStage %
                Delay *= 1 + _slowPerStage;

                _clogVisual.Play(true);

                // Debug.Log($"Clog [{_currentClogStage}] {gameObject.name}");
            }

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
            if (_currentClogStage == _stagesToBreak)
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

        public void OnClick(Vector3 hitPoint)
        {
        }

        public void OnPress(Vector3 hitPoint)
        {
            Repair();
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