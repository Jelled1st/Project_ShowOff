using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Factory
{
    public class FactoryQuestController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _level1Ui;

        [SerializeField]
        private GameObject _level2Ui;

        [Required]
        [SerializeField]
        private Image _potatoesCollectedImage;

        [Required]
        [SerializeField]
        private Sprite _potatoesCollectedImageLevel1;

        [Required]
        [SerializeField]
        private Sprite _potatoesCollectedImageLevel2;

        [SerializeField]
        private TextMeshProUGUI _washText;

        [SerializeField]
        private TextMeshProUGUI _peelText;

        [SerializeField]
        private TextMeshProUGUI _cutText;

        [SerializeField]
        private TextMeshProUGUI _packText;

        [SerializeField]
        private UnityEvent _washed;

        [SerializeField]
        private UnityEvent _peeled;

        [SerializeField]
        private UnityEvent _cutt;

        [SerializeField]
        private UnityEvent _packed;


        private void OnEnable()
        {
            Machine.ItemLeftMachine += SetPassedQuest;
        }

        private void OnDisable()
        {
            Machine.ItemLeftMachine -= SetPassedQuest;
        }

        public void SetLevel(int level)
        {
            switch (level)
            {
                case 1:
                    _level1Ui.SetActive(true);
                    _level2Ui.SetActive(false);
                    _potatoesCollectedImage.sprite = _potatoesCollectedImageLevel1;
                    break;
                case 2:
                    _level1Ui.SetActive(false);
                    _level2Ui.SetActive(true);
                    _potatoesCollectedImage.sprite = _potatoesCollectedImageLevel2;
                    break;
            }
        }

        private void SetPassedQuest(Machine.MachineType machineType)
        {
            switch (machineType)
            {
                case Machine.MachineType.PotatoWasher:
                    KitchenSubTutorial.StrikeThroughText(_washText);
                    _washed.Invoke();
                    break;
                case Machine.MachineType.PotatoPeeler:
                    KitchenSubTutorial.StrikeThroughText(_peelText);
                    _peeled.Invoke();
                    break;
                case Machine.MachineType.FryPacker:
                    KitchenSubTutorial.StrikeThroughText(_packText);
                    _cutt.Invoke();
                    break;
                case Machine.MachineType.FryCutter:
                    KitchenSubTutorial.StrikeThroughText(_cutText);
                    _packed.Invoke();
                    break;
            }
        }
    }
}