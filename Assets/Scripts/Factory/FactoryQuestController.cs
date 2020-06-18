using TMPro;
using UnityEngine;

namespace Factory
{
    public class FactoryQuestController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _level1Ui;

        [SerializeField]
        private GameObject _level2Ui;

        [SerializeField]
        private TextMeshProUGUI _washText;

        [SerializeField]
        private TextMeshProUGUI _peelText;

        [SerializeField]
        private TextMeshProUGUI _cutText;

        [SerializeField]
        private TextMeshProUGUI _packText;

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
                    break;
                case 2:
                    _level1Ui.SetActive(false);
                    _level2Ui.SetActive(true);
                    break;
            }
        }

        private void SetPassedQuest(Machine.MachineType machineType)
        {
            switch (machineType)
            {
                case Machine.MachineType.PotatoWasher:
                    KitchenSubTutorial.StrikeThroughText(_washText);
                    break;
                case Machine.MachineType.PotatoPeeler:
                    KitchenSubTutorial.StrikeThroughText(_peelText);
                    break;
                case Machine.MachineType.FryPacker:
                    KitchenSubTutorial.StrikeThroughText(_packText);
                    break;
                case Machine.MachineType.FryCutter:
                    KitchenSubTutorial.StrikeThroughText(_cutText);
                    break;
            }
        }
    }
}