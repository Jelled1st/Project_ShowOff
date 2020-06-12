using UnityEngine;

namespace Timeout
{
    [CreateAssetMenu]
    public class TimeoutOverlay : ScriptableObject
    {
        [field: SerializeField] public GameObject OverlayPrefab { get; private set; }
    }
}