using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "LanguagePack", menuName = "ScriptableObjects/LanguagePack", order = 1)]
public class LanguagePack : ScriptableObject
{
    [SerializeField] TextAsset _file;

    private LanguageHandler _languageHandler;

    public void UnPack(LanguageHandler handler)
    {
        _languageHandler = handler;
        string file = _file.text;
        string[] lines = file.Split('\n');
        for(int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            if(line.StartsWith("main."))
            {
                ParseLineMain(line);
            }
            else if(line.StartsWith("farm."))
            {

            }
        }
    }

    private void ParseLineMain(string line)
    {
        if (line.StartsWith("main.start: "))
        {
            _languageHandler.main.start = line.Remove(0, 12);
        }
        else if(line.StartsWith("main.exit: "))
        {
            _languageHandler.main.exit = line.Remove(0, 11);
        }
    }

    private void ParseLineFarm(string line)
    {

    }
}
