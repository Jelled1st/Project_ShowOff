using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

        [BoxGroup("Stage settings")]
        [SerializeField]
        [Tag]
        private string _allowedTruckObjectTag;

        [BoxGroup("Stage settings")]
        [SerializeField]
        [Scene]
        private string _nextScene;

        private float _initialScore;
        private bool _canAppendScore;
        private int _potatoesInput;
        private bool _level1Passed;
        private FactoryUiManager _factoryUiManager;
        private StageTimer _stageTimer;

        bool _trulyFinished = false;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _trulyFinished = true;
            }
        }

        private void Awake()
        {
            StartCoroutine(LoadScene());
            _factoryUiManager = FindObjectOfType<FactoryUiManager>();
            _stageTimer = FindObjectOfType<StageTimer>();
            _initialScore = Scores.GetCurrentScore();
        }

        private void OnEnable()
        {
            _level2Machines.ToggleChildren<Machine>(false);

            _finishTriggerLevel1.FinishTriggerHit += OnLevel1TriggerHit;
            _finishTriggerLevel2.FinishTriggerHit += OnLevel2TriggerHit;

            _peeledPotatoSpawner.enabled = false;

            _stageTimer.TimeEnded += OnTimeEnded;
            _stageTimer.StartTimer();

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene arg0)
        {
            if (!_canAppendScore)
                Scores.AddScore(Scores.GetCurrentScore() - _initialScore);

            Time.timeScale = 1f;
        }

        private void OnDisable()
        {
            _finishTriggerLevel1.FinishTriggerHit -= OnLevel1TriggerHit;
            _finishTriggerLevel2.FinishTriggerHit -= OnLevel2TriggerHit;

            _stageTimer.TimeEnded += OnTimeEnded;

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
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
            StartCoroutine(LoadScene());
            if (!obj.CompareTag(_allowedTruckObjectTag))
                return;

            PotatoesInput++;

            if (PotatoesInput >= _potatoesNeededToPassLevel2)
            {
                _canAppendScore = true;
                FindObjectOfType<BKM>().TruckDriving();

                //SceneManager.LoadScene(_nextScene);
                _trulyFinished = true;
            }
        }
        IEnumerator LoadScene()
        {
            yield return null;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_nextScene);

            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                // print("Loading Progress: " + (asyncOperation.progress * 100) + "%");
                if (asyncOperation.progress >= 0.9f)
                {
                    if (_trulyFinished == true)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                }
                yield return null;
            }
        }
    }
}