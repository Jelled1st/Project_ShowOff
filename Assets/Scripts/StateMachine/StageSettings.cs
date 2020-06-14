using NaughtyAttributes;
using UnityEngine;

public abstract class StageSettings : ScriptableObject
{
    [field: SerializeField, Scene] public int StageScene { get; private set; }
}