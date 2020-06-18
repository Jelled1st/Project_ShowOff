using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CuttableFallOffOptions", menuName = "ScriptableObjects/CuttableFallOffOptions", order = 1)]
public class CuttableFallOffOptions : ScriptableObject
{
    [SerializeField] List<GameObject> _fallOffPieces;
    [SerializeField] List<float> _fallOffPiecesLengths;
    private int _lastIndex = -1;

    public GameObject GetFallOffPiece()
    {
        _lastIndex = Random.Range(0, _fallOffPieces.Count);
        return _fallOffPieces[_lastIndex];
    }

    public float GetFallOffPieceLength()
    {
        return _fallOffPiecesLengths[_lastIndex];
    }
}
