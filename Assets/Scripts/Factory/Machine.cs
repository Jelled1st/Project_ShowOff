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

        public static event Action<MachineType, GameObject> ItemEnteredMachine = delegate { };
        public static event Action<MachineType> ItemLeftMachine = delegate { };
        public static event Action MachineStartedRepairing = delegate { };
        public static event Action<Machine> MachineRepaired = delegate { };
        public static event Action<Machine> MachineBreaking = delegate { };
        public static event Action<Machine> MachineBroke = delegate { };


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
        private float _slowPerStage = 1f / StagesToBreak;

        [BoxGroup("Clogging settings")]
        [MinMaxSlider(1f, 20f)]
        [SerializeField]
        private Vector2 _breakEverySeconds = new Vector2(4, 8);

        private const int StagesToBreak = 3;

        private Animator _machineAnimator;
        private int _currentClogStage;

        private bool _isRepairing;
        private readonly Queue<GameObject> _itemBuffer = new Queue<GameObject>();
        private Tween _releaseItemsTween;
        private CollisionCallback _collisionCallback;

        private int CurrentClogStage
        {
            get => _currentClogStage;
            set
            {
                if (value == StagesToBreak)
                {
                    Scores.AddScore(Scores.MachineCompleteBreakage);
                    MachineBroke(this);
                    _machineAnimator?.SetBool("isPlaying", false);

                    _releaseItemsTween?.Pause();
                }
                else
                {
                    _machineAnimator?.SetBool("isPlaying", true);

                    if (_releaseItemsTween != null && _releaseItemsTween.IsPlaying())
                        _releaseItemsTween?.Play();
                }

                // If WAS broken, then release items
                if (value == StagesToBreak - 1)
                {
                    ReleaseBufferedItems();
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

        private Queue<GameObject> ItemBuffer
        {
            get
            {
                if (_releaseItemsTween == null || !_releaseItemsTween.IsPlaying())
                    ReleaseBufferedItems();
                return _itemBuffer;
            }
        }

        private float Delay { get; set; }

        private bool IsClogged => CurrentClogStage == StagesToBreak;

        // If we later need it - it's the filtration by tag
        [SerializeField]
        private string _expectedInputTag;

        [SerializeField]
        private string _outputTag;
        // protected string AllowedInputTag => _expectedInputTag;
        // protected string OutputTag => _outputTag;

        protected abstract GameObject PreDelayProcess(GameObject inputGameObject);
        protected abstract GameObject PostDelayProcess(GameObject outputGameObject);

        private void OnEnable()
        {
            // Initial reset
            Delay = _delay;
            CurrentClogStage = 0;
            _isRepairing = false;

            _machineAnimator = GetComponentInChildren<Animator>();
            if (_machineAnimator == null)
                Debug.LogWarning("Animator not found!", this);

            // Set input trigger callback
            _collisionCallback = null;
            if (!_inputFunnelTrigger.gameObject.TryGetComponent(out _collisionCallback))
            {
                // Debug.LogWarning(
                //     $"[{gameObject.name}] lacks {nameof(CollisionCallback)} script! Edit the prefab! Trying to add it...");
                _collisionCallback = _inputFunnelTrigger.gameObject.AddComponent<CollisionCallback>();
            }

            _collisionCallback.TriggerEnter += TriggerEnter;


            _processParticles = _processParticles.NullIfEqualsNull();
            _processParticles?.Stop();

            _repairVisuals = _repairVisuals.NullIfEqualsNull();
            _repairVisuals?.SetActive(false);

            _breakVisuals = _breakVisuals.NullIfEqualsNull();
            _breakVisuals?.SetActive(false);

            WaitAndClog();
        }

        private void OnDisable()
        {
            _collisionCallback.TriggerEnter -= TriggerEnter;

            _processParticles?.Stop();
            _repairVisuals?.SetActive(false);
            _breakVisuals?.SetActive(false);
            _isRepairing = false;

            _waitAndClogTween?.Kill();
            _waitAndClogTween = null;
        }

        private void Clog()
        {
            if (!enabled || _isRepairing || IsClogged)
                return;

            CurrentClogStage++;

            // Increase delay by slowPerStage %
            Delay *= 1 + _slowPerStage;

            _clogVisual.Play(true);
            MachineBreaking(this);

            // Debug.Log($"Clog [{CurrentClogStage}] {gameObject.name}");

            WaitAndClog();
        }

        private Tween _waitAndClogTween;

        private void WaitAndClog()
        {
            _waitAndClogTween = DOTween.Sequence()
                .AppendInterval(Random.Range(_breakEverySeconds.x, _breakEverySeconds.y))
                .AppendCallback(Clog);
        }

        private void Repair()
        {
            if (CurrentClogStage <= 0 || _isRepairing || !enabled)
                return;

            _repairVisuals?.SetActive(true);
            _isRepairing = true;
            _waitAndClogTween.Kill();

            MachineStartedRepairing();

            // Debug.Log($"Started repairing {gameObject.name}");
            DOTween.Sequence().AppendInterval(_baseFixTime * CurrentClogStage)
                .AppendCallback(() =>
                {
                    // Debug.Log($"Finished repairing {gameObject.name}");
                    CurrentClogStage--;
                    _repairVisuals?.SetActive(false);
                    _isRepairing = false;
                    MachineRepaired(this);

                    // Decrease delay by slowPerStage %
                    Delay /= 1 + _slowPerStage;

                    WaitAndClog();
                });
        }

        private void ReleaseBufferedItems()
        {
            if (IsClogged) return;

            _releaseItemsTween = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    if (_itemBuffer.Count > 0)
                        PostDelayProcessInternal(_itemBuffer.Dequeue());
                })
                .AppendInterval(_delay)
                .SetLoops(_itemBuffer.Count);

            _releaseItemsTween.OnComplete(() => { _releaseItemsTween = null; });
        }

        private void TriggerEnter(Collider other)
        {
            StartCoroutine(Process(other.gameObject, Delay));
        }

        private IEnumerator Process(GameObject otherGameObject, float delay)
        {
            var check = otherGameObject.layer == LayerMask.NameToLayer("ConveyorBelt");
            if (check)
                yield break;

            var processedObject = PreDelayProcessInternal(otherGameObject);
            processedObject.SetActive(false);

            _processParticles?.Play();
            yield return new WaitForSeconds(delay);
            _processParticles?.Stop();

            ItemBuffer.Enqueue(processedObject);
        }

        private GameObject PreDelayProcessInternal(GameObject inputGameObject)
        {
            ItemEnteredMachine(_machineType, inputGameObject);
            return PreDelayProcess(inputGameObject);
        }

        private void PostDelayProcessInternal(GameObject processedObject)
        {
            ItemLeftMachine(_machineType);

            var processedItem = PostDelayProcess(processedObject);

            processedItem.SetActive(true);

            // Reset and spit the rigidbody
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