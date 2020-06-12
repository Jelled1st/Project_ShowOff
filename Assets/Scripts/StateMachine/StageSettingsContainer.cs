using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class StageSettingsContainer : ScriptableObject
    {
        [SerializeField] private StageSettings[] _stageSettings;

        public StageSettings[] StageSettings => _stageSettings;

        // Checks if two presets for the same stage are in the list
        private void OnValidate()
        {
            HashSet<Type> settingsTypes = new HashSet<Type>();

            foreach (var settings in _stageSettings)
            {
                if (!settingsTypes.Add(settings.GetType()))
                {
                    Debug.LogError(
                        $"{nameof(StageSettingsContainer)} contains more than one instance of {settings.GetType()} !");
                }
            }
        }
    }
}