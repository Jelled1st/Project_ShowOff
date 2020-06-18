using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using System.Collections;
using System.Linq;
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
        [ReorderableList]
        [SerializeField]
        private GameObject[] _potatoPackageStacks;

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
        [Required]
        private BufferBelt _bufferBelt;

        [BoxGroup("Scene objects")]
        [SerializeField]
        [Required]
        private Transform _truck;

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
        [Tag]
        private string _allowedLevel1ObjectTag;

        [BoxGroup("Stage settings")]
        [SerializeField]
        [Scene]
        private string _nextScene;

        [BoxGroup("Stage settings")]
        [SerializeField]
        private float _changeSceneAfterDriveInterval = 5f;

        private float _initialScore;
        private bool _canAppendScore;
        private int _potatoesInput;
        private bool _level1Passed;
        private bool _level2Passed;
        private FactoryUiManager _factoryUiManager;
        private StageTimer _stageTimer;
        private FactoryQuestController _factoryQuestController;

        bool _trulyFinished = false;

        public int PotatoesNeededToPassLevel1 => _potatoesNeededToPassLevel1;

        private int PotatoesInput
        {
            get => _potatoesInput;
            set
            {
                _potatoesInput = value;
                var of = _level1Passed ? _potatoesNeededToPassLevel2 : _potatoesNeededToPassLevel1;
                _factoryUiManager.SetPotatoesCount(_potatoesInput, of);
            }
        }

        private void Awake()
        {
            StartCoroutine(LoadScene());

            _factoryUiManager = FindObjectOfType<FactoryUiManager>();
            _stageTimer = FindObjectOfType<StageTimer>();
            _factoryQuestController = FindObjectOfType<FactoryQuestController>();

            _initialScore = Scores.GetCurrentScore();
        }

        private void OnEnable()
        {
            _potatoPackageStacks.ToList().ForEach(t => t.SetActive(false));

            _level2Machines.ToggleChildren<Machine>(false);

            _finishTriggerLevel1.FinishTriggerHit += OnLevel1TriggerHit;
            _finishTriggerLevel2.FinishTriggerHit += OnLevel2TriggerHit;

            _peeledPotatoSpawner.enabled = false;

            _stageTimer.TimeEnded += OnTimeEnded;
            _stageTimer.StartTimer();

            _factoryQuestController.SetLevel(1);

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

            _stageTimer.TimeEnded -= OnTimeEnded;

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnTimeEnded()
        {
            _factoryUiManager.ToggleLoseScreen(true);
        }


        private void OnLevel1TriggerHit(GameObject obj)
        {
            if (!obj.CompareTag(_allowedLevel1ObjectTag))
                return;

            PotatoesInput++;

            if (!_level1Passed && PotatoesInput == PotatoesNeededToPassLevel1)
            {
                _level1Passed = true;

                _factoryQuestController.SetLevel(2);

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

            var objTag = obj.tag;
            Destroy(obj);

            if (objTag != _allowedTruckObjectTag ||
                PotatoesInput >= _potatoesNeededToPassLevel2)
                return;

            if (PotatoesInput < _potatoPackageStacks.Length)
                _potatoPackageStacks[PotatoesInput].SetActive(true);

            PotatoesInput++;
            Scores.AddScore(Scores.LoadedPotatoBagged);

            if (PotatoesInput >= _potatoesNeededToPassLevel2)
            {
                _level2Passed = true;

                FinishScene();
            }
        }

        private void FinishScene()
        {
            _canAppendScore = true;
            var bkm = FindObjectOfType<BKM>();
            bkm.StopMusicFade();
            bkm.TruckDriving();
            _stageTimer.StopTimer();
            Scores.AddScore(Scores.LeftTimeMultiplier * _stageTimer.TimeRemaining);


            DOTween.Sequence()
                .Join(_truck.DOMove(_truck.position + _truck.right * 20f, _changeSceneAfterDriveInterval)
                    .SetEase(Ease.InQuint))
                .AppendInterval(_changeSceneAfterDriveInterval)
                .AppendCallback(() =>
                {
                    print("should change scene");
                    _trulyFinished = true;
                });
        }

        private AsyncOperation sceneLoad;

        IEnumerator LoadScene()
        {
            yield return null;

            if (SceneManager.GetSceneByName(_nextScene).isLoaded || sceneLoad != null)
                yield break;

            sceneLoad = SceneManager.LoadSceneAsync(_nextScene);

            sceneLoad.allowSceneActivation = false;

            while (!sceneLoad.isDone)
            {
                // print("Loading Progress: " + (asyncOperation.progress * 100) + "%");
                if (sceneLoad.progress >= 0.9f)
                {
                    if (_trulyFinished == true)
                    {
                        sceneLoad.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }
    }
}