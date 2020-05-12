using System;
using System.Linq;
using Factory;
using UnityEngine;

namespace StateMachine
{
    public class GameStageManager : LazySingleton<GameStageManager>
    {
        [SerializeField] private StageSettingsContainer _stageSettingsContainer;
        [SerializeField] private GameStages _initialState;

        private static GameStage CurrentStageType { get; set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            Debug.Log($"Changing stage to {{{_initialState}}}");
            switch (_initialState)
            {
                case GameStages.Factory:
                    ChangeStage<FactoryStage>();
                    break;
                default:
                    throw new Exception(
                        $"Stage {{{_initialState}}} is either not implemented or hasn't been added to GameStageManager");
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
            CurrentStageType?.Cleanup();

            CurrentStageType = new T();
            Instance.StartCoroutine(CurrentStageType.Setup());
        }
    }
}