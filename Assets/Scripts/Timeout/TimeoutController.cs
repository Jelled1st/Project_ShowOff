using System;
using System.Linq;
using System.Timers;
using DG.Tweening;
using StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Timer = System.Timers.Timer;


namespace Timeout
{
    public class TimeoutController : LazySingleton<TimeoutController>
    {
        public const float TimeoutInterval = 3f;

        private static Sequence _timer;

        private static bool CanOpenTimeoutScreen
        {
            get
            {
                switch (SceneManager.GetActiveScene().name)
                {
                    case "!Main Menu":
                    case "!Ending Scene":
                        return false;
                    default:
                        return true;
                }
            }
        }

        private static GameObject _overlay;

        private static GameObject Overlay
        {
            get
            {
                if (_overlay == null || _overlay.Equals(null))
                {
                    var overlay = Resources.FindObjectsOfTypeAll<TimeoutOverlay>().FirstOrDefault();

                    if (overlay != null)
                    {
                        _overlay = Instantiate(overlay.OverlayPrefab);
                    }
                }

                return _overlay;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private static void OnTimeout()
        {
            if (!CanOpenTimeoutScreen)
            {
                return;
            }

            _timer.Pause();

            SetPause(true);

            Overlay.SetActive(true);
        }

        public void Continue()
        {
            ResetTimer();

            SetPause(false);

            Overlay.SetActive(false);
        }

        public static void SetPause(bool pause)
        {
            if (pause)
            {
                switch (SceneManager.GetActiveScene().name)
                {
                    case "!Farm Level":
                        FindObjectOfType<FarmGameHandler>().Pause(false);
                        break;
                    case "!Factory Level":
                        Time.timeScale = 0;
                        break;
                    case "!Kitchen Level":
                        FindObjectOfType<KitchenGameHandler>().Pause(false);
                        break;
                }
            }
            else
            {
                switch (SceneManager.GetActiveScene().name)
                {
                    case "!Farm Level":
                        FindObjectOfType<FarmGameHandler>().UnPause();
                        break;
                    case "!Factory Level":
                        Time.timeScale = 1f;
                        break;
                    case "!Kitchen Level":
                        FindObjectOfType<KitchenGameHandler>().UnPause();
                        break;
                }
            }
        }

        public static void ResetTimer()
        {
            _timer.Restart();
        }

        public static void StopTimer()
        {
            _timer.Rewind();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _timer = DOTween.Sequence().AppendInterval(TimeoutInterval)
                .AppendCallback(OnTimeout).SetAutoKill(false);

            ResetTimer();
            DontDestroyOnLoad(Instance);
        }


        private void Update()
        {
            if (Input.anyKey)
            {
                ResetTimer();
            }
        }
    }
}