using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

        public void Start()
        {
            Setup();
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     Camera.main.transform.DOMove(_level2CameraPosition.position, 2f);
            //     Camera.main.transform.DORotate(_level2CameraPosition.rotation.eulerAngles, 2f);
            //
            //     _shadowPlane.transform.DOMove(_level2ShadowPlanePosition.position, 2f);
            //     _shadowPlane.transform.DORotate(_level2ShadowPlanePosition.rotation.eulerAngles, 2f);
            // }
        }


        private void Setup()
        {
            _level2Machines.ToggleChildren<Machine>(false);

            _finishTriggerLevel1.FinishTriggerHit += OnLevel1TriggerHit;
            _finishTriggerLevel2.FinishTriggerHit += OnLevel2TriggerHit;

            _peeledPotatoSpawner.enabled = false;
        }


        private void OnLevel1TriggerHit(GameObject obj)
        {
            _potatoesInput++;

            if (!_level1Passed && _potatoesInput == _potatoesNeededToPassLevel1)
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

                _potatoesInput = 0;
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
            _potatoesInput++;

            if (_potatoesInput >= _potatoesNeededToPassLevel2)
            {
                SceneManager.LoadScene("!Ending Scene");
            }
        }
    }
}