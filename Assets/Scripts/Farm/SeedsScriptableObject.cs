using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seeds", menuName = "ScriptableObjects/FarmSeeds", order = 1)]
public class SeedsScriptableObject : ScriptableObject
{
    [SerializeField] private string _file;
    [SerializeField] private GameObject _cropPrefab;

    public string GetFile()
    {
        return _file;
    }
}
