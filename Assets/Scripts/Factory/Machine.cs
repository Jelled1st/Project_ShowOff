using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Factory
{
    [RequireComponent(typeof(Collider))]
    [SelectionBase]
    public abstract class Machine : MonoBehaviour, IControllable
    {
        public enum MachineType
        {
            PotatoWasher,
            PotatoPeeler,
            FryPacker,
            FryCutter
        }

        public static event Action<MachineType> ItemEnteredMachine = delegate { };
        public static event Action<MachineType> ItemLeftMachine = delegate { };
        public static event Action MachineStartedRepairing = delegate { };
        public static event Action MachineBreaking = delegate { };
        public static event Action MachineBroke = delegate { };


        [BoxGroup("Machine settings")]
        [SerializeField]
        private MachineType _machineType;

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

        [FormerlySerializedAs("_particleSystem")]
        [BoxGroup("Processing settings")]
        [SerializeField]
        private ParticleSystem _processParticles;

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
        [SerializeField]
        private GameObject _breakVisuals;

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

        private int CurrentClogStage
        {
            get => _currentClogStage;
            set
            {
                if (value == _stagesToBreak)
                {
                    Scores.AddScore(Scores.MachineCompleteBreakage);
                    MachineBroke();
                }

                // If WAS broken, then release items
                if (value == _stagesToBreak - 1)
                {
                    ReleaseBufferedItems();
                    MachineBreaking();
                }

                if (value > 1)
                {
                    _breakVisuals?.SetActive(true);
                }

                if (value == 1)
                {
                    _breakVisuals.SetActive(false);
                }

                _currentClogStage = value;
            }
        }

        private bool _isRepairing;
        private Queue<GameObject> _itemBuffer = new Queue<GameObject>();

        private float Delay { get; set; }

        private bool IsClogged => CurrentClogStage == _stagesToBreak;

        // If we later need it - it's the filtration by tag
        // [SerializeField] private string _allowedInputTag;
        // [SerializeField] private string _outputTag;
        // protected string AllowedInputTag => _allowedInputTag;
        // protected string OutputTag => _outputTag;

        protected abstract GameObject PreDelayProcess(GameObject inputGameObject);
        protected abstract GameObject PostDelayProcess(GameObject outputGameObject);

        private void Start()
        {
            // Initial reset
            Delay = _delay;
            CurrentClogStage = 0;
            _isRepairing = false;

            // Set input trigger callback
            if (!TryGetComponent(out CollisionCallback collisionCallback))
            {
                // Debug.LogWarning(
                //     $"[{gameObject.name}] lacks {nameof(CollisionCallback)} script! Edit the prefab! Trying to add it...");
            }

            collisionCallback = _inputFunnelTrigger.gameObject.AddComponent<CollisionCallback>();
            collisionCallback.onTriggerEnter += OnTriggerEnterCallback;

            _processParticles = _processParticles.NullIfEqualsNull();
            _processParticles?.Stop();

            _repairVisuals = _repairVisuals.NullIfEqualsNull();
            _repairVisuals?.SetActive(false);

            _breakVisuals = _breakVisuals.NullIfEqualsNull();
            _breakVisuals?.SetActive(false);

            WaitAndClog();
        }

        private void Clog()
        {
            if (_isRepairing || IsClogged)
                return;

            CurrentClogStage++;

            // Increase delay by slowPerStage %
            Delay *= 1 + _slowPerStage;

            _clogVisual.Play(true);

            // Debug.Log($"Clog [{CurrentClogStage}] {gameObject.name}");

            WaitAndClog();
        }

        private void Repair()
        {
            if (CurrentClogStage <= 0 || _isRepairing)
                return;

            _repairVisuals?.SetActive(true);
            _isRepairing = true;
            MachineStartedRepairing();

            // Debug.Log($"Started repairing {gameObject.name}");
            DOTween.Sequence().AppendInterval(_baseFixTime * CurrentClogStage)
                .AppendCallback(() =>
                {
                    // Debug.Log($"Finished repairing {gameObject.name}");
                    CurrentClogStage--;
                    _repairVisuals?.SetActive(false);
                    _isRepairing = false;

                    // Decrease delay by slowPerStage %
                    Delay /= 1 + _slowPerStage;

                    WaitAndClog();
                });
        }

        private void ReleaseBufferedItems()
        {
            if (_itemBuffer.Count > 0)
            {
                DOTween.Sequence().AppendCallback(() => { PostDelayProcessInternal(_itemBuffer.Dequeue()); })
                    .AppendInterval(_delay).SetLoops(_itemBuffer.Count);
            }
        }

        private void WaitAndClog()
        {
            DOTween.Sequence().AppendInterval(Random.Range(_breakEverySeconds.x, _breakEverySeconds.y))
                .AppendCallback(Clog);
        }

        private void OnTriggerEnterCallback(Collider other)
        {
            StartCoroutine(Process(other.gameObject, Delay));
        }


        private IEnumerator Process(GameObject otherGameObject, float delay)
        {
            if (IsClogged)
                yield break;

            var check = otherGameObject.layer == LayerMask.NameToLayer("ConveyorBelt");
            if (check)
                yield break;

            var processedObject = PreDelayProcessInternal(otherGameObject);

            _processParticles?.Play();
            yield return new WaitForSeconds(delay);
            _processParticles?.Stop();

            PostDelayProcessInternal(processedObject);
        }

        private GameObject PreDelayProcessInternal(GameObject inputGameObject)
        {
            ItemEnteredMachine(_machineType);
            return PreDelayProcess(inputGameObject);
        }

        private void PostDelayProcessInternal(GameObject processedObject)
        {
            ItemLeftMachine(_machineType);
            // Reset and spit the rigidbody
            var processedItem = PostDelayProcess(processedObject);

            if (processedItem.TryGetComponent(out Rigidbody spitItem))
            {
                spitItem.velocity = Vector3.zero;
                spitItem.angularVelocity = Vector3.zero;
                spitItem.transform.position = _output.position;
                spitItem.AddForce(_outputPushForce * _output.right);
            }
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