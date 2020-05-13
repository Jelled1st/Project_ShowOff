using System.Collections;

namespace StateMachine
{
    public abstract class GameStage<T> : GameStage where T : StageSettings
    {
        protected static T StageSettings => GameStageManager.GetStageSettings<T>();
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