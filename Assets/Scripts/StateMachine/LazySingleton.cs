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
            if (_instance != null && !_instance.Equals(null)) return;

            var singletons = FindObjectsOfType<T>();
            if (singletons.Length == 1)
            {
                _instance = singletons.First();
            }

            if (singletons.Length > 1)
            {
                throw new Exception($"More than one object of type {{{typeof(T)}}} is present in scene!");
            }

            if (singletons.Length == 0)
            {
                _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
            }
        }
    }
}