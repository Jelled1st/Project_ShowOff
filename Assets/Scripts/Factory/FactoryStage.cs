using System.Collections;
using DG.Tweening;
using StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Factory
{
    public class FactoryStage : GameStage<FactoryStageSettings>
    {
        private GameObject _package;
        private bool _isRespawning;
        private Vector3 _lastPosition;
        private int _potatoesInput;
        private bool _level1Passed;

        private TempFactoryController _tempFactoryController;

        public override void Setup()
        {
            SceneManager.LoadScene(StageSettings.StageScene);

            FinishTrigger.FinishTriggerHit += OnFinishTriggerHit;
        }

        protected override void OnSceneLoaded()
        {
            _tempFactoryController = Object.FindObjectOfType<TempFactoryController>();
        }

        public override void Cleanup()
        {
            FinishTrigger.FinishTriggerHit -= OnFinishTriggerHit;
        }

        private void OnFinishTriggerHit(GameObject hitGameObject)
        {
            _potatoesInput++;
            Debug.Log(_potatoesInput);
            if (!_level1Passed && _potatoesInput == 9)
            {
                _level1Passed = true;
                Camera.main.transform.DOMove(_tempFactoryController.level2CameraPosition.position, 2f);
                Camera.main.transform.DORotate(_tempFactoryController.level2CameraPosition.rotation.eulerAngles, 2f);
                _tempFactoryController.level1Machines.SetActive(false);
                _tempFactoryController.level2Machines.SetActive(true);
                _tempFactoryController.bufferBelts[0].Speed = -1f;
                _tempFactoryController.bufferBelts[1].Speed = 1f;
                foreach (var flatConveyorBelt in _tempFactoryController.level1Belts)
                {
                    flatConveyorBelt.Speed = 0f;
                }
                _tempFactoryController.spawner.GetComponent<ObjectSpawner>().StopSpawning();
                Object.Destroy(_tempFactoryController.spawner);
                _potatoesInput = 0;
                _tempFactoryController.finishTriggerLevel1.SetActive(false);
                
            }

            if (_level1Passed)
            {
                SceneManager.LoadScene(2);
            }
        }

        private void LevelFinished()
        {
            Debug.Log("Package reached the end");
            //TODO: Change stage
        }
    }
}