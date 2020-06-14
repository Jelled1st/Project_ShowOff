using System;
using StateMachine;
using TMPro;
using UnityEngine;

namespace Factory
{
    public class FactoryUiManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _timerText;

        [SerializeField]
        private TextMeshPro _potatoesCollectedText;

        [SerializeField]
        private GameObject _youLostScreen;

        public void ToggleLoseScreen(bool toggle)
        {
            _youLostScreen.SetActive(toggle);
        }

        public void SetTimer(float time)
        {
            _timerText.text = time.ToString();
        }

        public void SetPotatoesCount(int collected, int of)
        {
            _potatoesCollectedText.text = $"{collected}/{of}";
        }
    }
}