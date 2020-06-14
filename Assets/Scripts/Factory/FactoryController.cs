using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Factory
{
    public class FactoryController : MonoBehaviour
    {
        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private FinishTrigger _finishTriggerLevel1;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private FinishTrigger _finishTriggerLevel2;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private ObjectSpawner _potatoSpawner;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private ObjectSpawner _peeledPotatoSpawner;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private Transform _level1Machines;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private Transform _level2Machines;

        [BoxGroup("Scene objects")]
        [SerializeField]
        private BufferBelt _bufferBelt;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private Transform _level2CameraPosition;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private GameObject _shadowPlane;

        [BoxGroup("Scene objects")]
        [Required]
        [SerializeField]
        private Transform _level2ShadowPlanePosition;

        [BoxGroup("Stage settings")]
        [SerializeField]
        private int _potatoesNeededToPassLevel1 = 9;

        [BoxGroup("Stage settings")]
        [SerializeField]
        private int _potatoesNeededToPassLevel2 = 3;

        private int _potatoesInput;
        private bool _level1Passed;
        private FactoryUiManager _factoryUiManager;
        private FactoryTimer _factoryTimer;

        public int PotatoesNeededToPassLevel1 => _potatoesNeededToPassLevel1;

        private int PotatoesInput
        {
            get => _potatoesInput;
            set
            {
                _potatoesInput = value;
                _factoryUiManager.SetPotatoesCount(_potatoesInput, _potatoesNeededToPassLevel1);
            }
        }

        private void Awake()
        {
            _factoryUiManager = FindObjectOfType<FactoryUiManager>();
            _factoryTimer = FindObjectOfType<FactoryTimer>();
        }

        public void Start()
        {
            Setup();
        }

        private void Setup()
        {
            _level2Machines.ToggleChildren<Machine>(false);

            _finishTriggerLevel1.FinishTriggerHit += OnLevel1TriggerHit;
            _finishTriggerLevel2.FinishTriggerHit += OnLevel2TriggerHit;

            _peeledPotatoSpawner.enabled = false;

            _factoryTimer.TimeEnded += OnTimeEnded;
            _factoryTimer.StartTimer();
        }

        private void OnTimeEnded()
        {
            _factoryUiManager.ToggleLoseScreen(true);
        }


        private void OnLevel1TriggerHit(GameObject obj)
        {
            PotatoesInput++;

            if (!_level1Passed && PotatoesInput == PotatoesNeededToPassLevel1)
            {
                _level1Passed = true;

                Camera.main.transform.DOMove(_level2CameraPosition.position, 2f);
                Camera.main.transform.DORotate(_level2CameraPosition.rotation.eulerAngles, 2f);

                _shadowPlane.transform.DOMove(_level2ShadowPlanePosition.position, 2f);
                _shadowPlane.transform.DORotate(_level2ShadowPlanePosition.rotation.eulerAngles, 2f);

                _level1Machines.ToggleChildren<Machine>(false);
                _level2Machines.ToggleChildren<Machine>(true);

                _potatoSpawner.enabled = false;

                _bufferBelt.ReleaseObjects();

                _bufferBelt.FinishedOutputting += OnBufferBeltFinishedSpawning;

                PotatoesInput = 0;
                _finishTriggerLevel1.gameObject.SetActive(false);
            }
        }


        private void OnBufferBeltFinishedSpawning()
        {
            _peeledPotatoSpawner.enabled = true;
            _bufferBelt.FinishedOutputting -= OnBufferBeltFinishedSpawning;
        }

        private void OnLevel2TriggerHit(GameObject obj)
        {
            PotatoesInput++;

            if (PotatoesInput >= _potatoesNeededToPassLevel2)
            {
                SceneManager.LoadScene("!Ending Scene");
            }
        }
    }
}