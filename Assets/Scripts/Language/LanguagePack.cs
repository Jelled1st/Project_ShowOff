using UnityEngine;

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
            else if(line.StartsWith("factory."))
            {
                ParseLineFactory(line);
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
            _languageHandler.main.exit = line.Substring(10, line.Length - 11);
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
    
    private void ParseLineFactory(string line)
    {
        if (line.StartsWith("factory.washQuest: "))
        {
            _languageHandler.factory.washQuest = line.Substring(18, line.Length - 19);
        }
        else if (line.StartsWith("factory.peelQuest: "))
        {
            _languageHandler.factory.peelQuest = line.Substring(18, line.Length - 19);
        }
        else if (line.StartsWith("factory.chopQuest: "))
        {
            _languageHandler.factory.chopQuest = line.Substring(18, line.Length - 19);
        }
        else if (line.StartsWith("factory.packageQuest: "))
        {
            _languageHandler.factory.packageQuest = line.Substring(21, line.Length - 22);
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
