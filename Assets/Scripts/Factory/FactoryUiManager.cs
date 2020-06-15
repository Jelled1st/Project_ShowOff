using System;
using StateMachine;
using TMPro;
using UnityEngine;

namespace Factory
{
    public class FactoryUiManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private TextMeshProUGUI _potatoesCollectedText;

        [SerializeField]
        private GameObject _youLostScreen;

        public void ToggleLoseScreen(bool toggle)
        {
            _youLostScreen.SetActive(toggle);
        }

        public void SetTimer(int time)
        {
            _timerText.text = time.ToString();
        }

        public void SetPotatoesCount(int collected, int of)
        {
            _potatoesCollectedText.text = $"{collected}/{of}";
        }
    }
}