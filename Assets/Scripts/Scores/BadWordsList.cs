using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class BadWordsList : ScriptableObject
{
    private const string BadWordsResource = "Bad Words";

    [SerializeField]
    private string[] _badWords;

    private static BadWordsList Instance
    {
        get
        {
            var badWordsList = Resources.Load<BadWordsList>(BadWordsResource);
            if (badWordsList == null)
            {
                Debug.LogError(
                    $"{nameof(BadWordsList)} can't find the resource file named {BadWordsResource} of type {nameof(BadWordsList)}");
            }

            return badWordsList;
        }
    }

    public static bool IsBadWord(string word)
    {
        return Instance._badWords.Contains(word.ToLower());
    }
}