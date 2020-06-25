using NaughtyAttributes;
using UnityEngine;

public abstract class StageSettings : ScriptableObject
{
    [field: SerializeField] [field: Scene] public int StageScene { get; private set; }
}