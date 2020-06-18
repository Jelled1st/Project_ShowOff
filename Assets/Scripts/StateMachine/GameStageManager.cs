using System;
using UnityEngine;

namespace StateMachine
{
    public class GameStageManager : LazySingleton<GameStageManager>
    {
        [SerializeField] private StageSettingsContainer _stageSettingsContainer;
        [SerializeField] private GameStages _initialStage;

        private static GameStage CurrentStageType { get; set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);


            StartInitialStage();
        }

        private void StartInitialStage()
        {
            if (CurrentStageType != null)
                return;

            switch (_initialStage)
            {
                case GameStages.Factory:
                    // ChangeStage<FactoryStage>();
                    break;
                default:
                    throw new Exception(
                        $"Stage {{{_initialStage}}} is either not implemented or hasn't been added to GameStageManager");
            }
        }

        public static T GetStageSettings<T>() where T : StageSettings
        {
            foreach (var settings in Instance._stageSettingsContainer.StageSettings)
            {
                if (settings.GetType() == typeof(T))
                {
                    return (T) settings;
                }
            }

            throw new Exception($"Can't find the stage settings of type {typeof(T)}");
        }

        public static void ChangeStage<T>() where T : GameStage, new()
        {
            Debug.Log($"Changing stage to {{{typeof(T)}}}");

            CurrentStageType?.Cleanup();

            CurrentStageType = new T();
            CurrentStageType.Setup();
        }
    }
}