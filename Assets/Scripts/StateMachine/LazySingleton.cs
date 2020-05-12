using System;
using System.Linq;
using UnityEngine;

namespace StateMachine
{
    public abstract class LazySingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        protected static T Instance
        {
            get
            {
                InitializeSingleton();

                return _instance;
            }
        }

        private static void InitializeSingleton()
        {
            if (_instance == null || _instance.Equals(null))
            {
                var singletons = FindObjectsOfType<T>();
                if (singletons.Length > 1)
                {
                    throw new Exception($"More than one object of type {{{typeof(T)}}} is present in scene!");
                }
                else if (singletons.Length == 1)
                {
                    _instance = singletons.First();
                }
                else
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
            }
        }
    }
}