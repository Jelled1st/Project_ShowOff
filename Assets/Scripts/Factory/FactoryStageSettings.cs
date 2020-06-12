using NaughtyAttributes;
using UnityEngine;

public class FactoryStageSettings : StageSettings
{
    [field: SerializeField] public GameObject PackagePrefab { get; private set; }
    [field: SerializeField] public float GroundLevel { get; private set; }
    [field: SerializeField] public float RespawnTime { get; private set; }
    [field: SerializeField] public float PushForce { get; private set; }
    [field: SerializeField] public float StuckCheckTime { get; private set; }
    [field: SerializeField] public float StuckDistance { get; private set; }
}