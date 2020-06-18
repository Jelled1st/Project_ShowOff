using System;
using UnityEngine.SceneManagement;

namespace StateMachine
{
    public abstract class GameStage<T> : GameStage, IDisposable where T : StageSettings
    {
        protected static T StageSettings => GameStageManager.GetStageSettings<T>();

        protected abstract void OnSceneLoaded();

        public GameStage()
        {
            SceneManager.sceneLoaded += OnSceneLoadedInternal;
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedInternal;
        }

        private void OnSceneLoadedInternal(Scene loadedScene, LoadSceneMode arg1)
        {
            if (loadedScene == SceneManager.GetSceneByBuildIndex(StageSettings.StageScene))
            {
                OnSceneLoaded();
            }
        }
    }

    public abstract class GameStage
    {
        public abstract void Cleanup();
        public abstract void Setup();

        public void RestartStage()
        {
            Cleanup();

            Setup();
        }
    }
}