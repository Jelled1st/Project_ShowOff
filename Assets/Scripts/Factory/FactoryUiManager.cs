using TMPro;
using UnityEngine;

namespace Factory
{
    public class FactoryUiManager : MonoBehaviour, ISetTimer
    {
        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private TextMeshProUGUI _potatoesCollectedText;

        [SerializeField]
        private GameObject _youLostScreen;

        private void OnEnable()
        {
            _youLostScreen = _youLostScreen.NullIfEqualsNull();
            _youLostScreen?.SetActive(false);
        }

        public void ToggleLoseScreen(bool toggle)
        {
            if (_youLostScreen != null)
            {
                _youLostScreen.SetActive(toggle);

                Time.timeScale = toggle ? 0f : 1f;
            }
            else
                Debug.LogError(
                    $"{nameof(FactoryUiManager)} does not contain reference to {nameof(_youLostScreen)}");
        }

        public void SetTimer(int time)
        {
            if (_timerText != null)
                _timerText.text = time.ToString();
            else
                Debug.LogError(
                    $"{nameof(FactoryUiManager)} does not contain reference to {nameof(_timerText)}");
        }

        public void SetPotatoesCount(int collected, int of)
        {
            if (_potatoesCollectedText != null)
                _potatoesCollectedText.text = $"{collected}/{of}";
            else
                Debug.LogError(
                    $"{nameof(FactoryUiManager)} does not contain reference to {nameof(_potatoesCollectedText)}");
        }
    }
}