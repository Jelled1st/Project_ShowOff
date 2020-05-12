using System.Collections;
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

        public override IEnumerator Setup()
        {
            SceneManager.LoadScene(StageSettings.StageScene);

            FinishTrigger.FinishTriggerHit += OnFinishTriggerHit;

            yield return null;
        }

        public override void Cleanup()
        {
            FinishTrigger.FinishTriggerHit -= OnFinishTriggerHit;
        }

        private void OnFinishTriggerHit(GameObject hitGameObject)
        {
            if (hitGameObject.CompareTag("Factory/Package"))
            {
                LevelFinished();
            }
        }

        private void LevelFinished()
        {
            Debug.Log("Package reached the end");
            //TODO: Change stage
        }
    }
}