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
            if (lines[i] == "") continue;
            string line = lines[i];
            if(line.StartsWith("main."))
            {
                ParseLineMain(line);
            }
            else if(line.StartsWith("farm."))
            {
                ParseLineFarm(line);
            }
            else if (line.StartsWith("kitchen."))
            {
                ParseLineKitchen(line);
            }
            else if (line.StartsWith("misc."))
            {
                ParseLineMisc(line);
            }
        }
    }

    private void ParseLineMain(string line)
    {
        if (line.StartsWith("main.start: "))
        {
            _languageHandler.main.start = line.Substring(11, line.Length - 12);
        }
        else if(line.StartsWith("main.exit: "))
        {
            _languageHandler.main.exit = line.Remove(10, line.Length - 11);
        }
    }

    private void ParseLineFarm(string line)
    {
        if (line.StartsWith("farm.mainQuest: "))
        {
            _languageHandler.farm.mainQuest = line.Substring(15, line.Length - 16);
        }
        else if (line.StartsWith("farm.shovelQuest: "))
        {
            _languageHandler.farm.shovelQuest = line.Substring(17, line.Length - 18);
        }
        else if (line.StartsWith("farm.plantQuest: "))
        {
            _languageHandler.farm.plantQuest = line.Substring(16, line.Length - 17);
        }
        else if (line.StartsWith("farm.waterQuest: "))
        {
            _languageHandler.farm.waterQuest = line.Substring(16, line.Length-17);
        }
    }

    private void ParseLineKitchen(string line)
    {
        if (line.StartsWith("kitchen.fryFries: "))
        {
            _languageHandler.kitchen.fryFries = line.Substring(17, line.Length-18);
        }
        else if (line.StartsWith("kitchen.cookPatty: "))
        {
            _languageHandler.kitchen.cookPatty = line.Substring(18, line.Length-19);
        }
    }

    private void ParseLineMisc(string line)
    {
        if (line.StartsWith("misc.score: "))
        {
            _languageHandler.misc.score = line.Substring(11, line.Length-12);
        }
    }
}
