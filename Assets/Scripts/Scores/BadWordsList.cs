using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu]
public class BadWordsList : ScriptableObject
{
    private const string BadWordsResource = "Bad Words";

    [SerializeField]
    private string[] _badWords;

    private string[] _customBadWords;

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

    [RuntimeInitializeOnLoadMethod]
    private static void LoadCustomBadWords()
    {
        try
        {
            var path = Path.Combine(Application.streamingAssetsPath, "CustomBadWords.txt");
            var customWords = UnityWebRequest.Get(path);
            customWords.SendWebRequest();

            var i = 0;
            while (!customWords.isDone && i++ < 1000)
            {
                Thread.Sleep(1);
            }

            Instance._customBadWords = customWords.downloadHandler.text.Split('\n')
                .Where(w => w != "\n" && w != "\r" && w.Length >= 3)
                .Select(w => w.ToLower().Replace("\r", "").Replace("\n", "")).ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static bool IsBadWord(string word)
    {
        return Instance._badWords.Contains(word.ToLower()) ||
               Instance._customBadWords != null && Instance._customBadWords.Contains(word.ToLower());
    }
}