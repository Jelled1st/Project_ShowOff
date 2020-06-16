using UnityEngine;

[CreateAssetMenu]
public class BadWordsList: ScriptableObject
{
    [SerializeField]
    private string[] _badWords;
    
    public string[] BadWords => _badWords;
}