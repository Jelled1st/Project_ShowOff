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
            if (lines[i].StartsWith("/"))
            {
                continue;
            }
            string line = lines[i];
            if (line.StartsWith("main."))
            {
                ParseLineMain(line);
            }
            else if (line.StartsWith("farm."))
            {
                ParseLineFarm(line);
            }
            else if (line.StartsWith("factory."))
            {
                ParseLineFactory(line);
            }
            else if (line.StartsWith("kitchen."))
            {
                ParseLineKitchen(line);
            }
            else if (line.StartsWith("misc.") || line.StartsWith("persUI.") || line.StartsWith("timeout."))
            {
                ParseLineMisc(line);
            }
            else Debug.LogError("Translation not found for: " + _file.name + "(" + line + ")"); 
        }
    }

    private void ParseLineMain(string line)
    {
        if (line.StartsWith("main.start: "))
        {
            _languageHandler.main.start = line.Substring(12, line.Length - 12);
        }
        else if(line.StartsWith("main.exit: "))
        {
            _languageHandler.main.exit = line.Substring(11, line.Length - 11);
        }
        else if (line.StartsWith("main.subtitle: "))
        {
            _languageHandler.main.subtitle = line.Substring(15, line.Length - 15);
        }
    }

    private void ParseLineFarm(string line)
    {
        if (line.StartsWith("farm.mainQuest: "))
        {
            _languageHandler.farm.mainQuest = line.Substring(16, line.Length - 16);
        }
        else if (line.StartsWith("farm.shovelQuest: "))
        {
            _languageHandler.farm.shovelQuest = line.Substring(18, line.Length - 18);
        }
        else if (line.StartsWith("farm.plantQuest: "))
        {
            _languageHandler.farm.plantQuest = line.Substring(17, line.Length - 17);
        }
        else if (line.StartsWith("farm.waterQuest: "))
        {
            _languageHandler.farm.waterQuest = line.Substring(17, line.Length-17);
        }
    }
    
    private void ParseLineFactory(string line)
    {
        if (line.StartsWith("factory.mainQuest: "))
        {
            _languageHandler.factory.mainQuest = line.Substring(19, line.Length - 19);
        }
        if (line.StartsWith("factory.level2Quest: "))
        {
            _languageHandler.factory.level2Quest = line.Substring(21, line.Length - 21);
        }
        else if (line.StartsWith("factory.washQuest: "))
        {
            _languageHandler.factory.washQuest = line.Substring(19, line.Length - 19);
        }
        else if (line.StartsWith("factory.peelQuest: "))
        {
            _languageHandler.factory.peelQuest = line.Substring(19, line.Length - 19);
        }
        else if (line.StartsWith("factory.chopQuest: "))
        {
            _languageHandler.factory.chopQuest = line.Substring(19, line.Length - 19);
        }
        else if (line.StartsWith("factory.packageQuest: "))
        {
            _languageHandler.factory.packageQuest = line.Substring(22, line.Length - 22);
        }
    }

    private void ParseLineKitchen(string line)
    {
        //8
        if (line.StartsWith("kitchen.cookPatty: "))
        {
            _languageHandler.kitchen.cookPatty = line.Substring(19, line.Length - 19);
        }
        else if (line.StartsWith("kitchen.cutVegetables: "))
        {
            _languageHandler.kitchen.cutVegetables = line.Substring(23, line.Length - 23);
        }
        else if (line.StartsWith("kitchen.assembleBurger: "))
        {
            _languageHandler.kitchen.assembleBurger = line.Substring(24, line.Length - 24);
        }
        else if (line.StartsWith("kitchen.dropMeat: "))
        {
            _languageHandler.kitchen.dropMeat = line.Substring(18, line.Length - 18);
        }
        else if (line.StartsWith("kitchen.dropVeggies: "))
        {
            _languageHandler.kitchen.dropVeggies = line.Substring(21, line.Length - 21);
        }
        else if (line.StartsWith("kitchen.stirPot: "))
        {
            _languageHandler.kitchen.stirPot = line.Substring(17, line.Length - 17);
        }
        else if (line.StartsWith("kitchen.assembleDish: "))
        {
            _languageHandler.kitchen.assembleDish = line.Substring(22, line.Length - 22);
        }
        else if (line.StartsWith("kitchen.bakeFish: "))
        {
            _languageHandler.kitchen.bakeFish = line.Substring(18, line.Length - 18);
        }
        else if (line.StartsWith("kitchen.stirSalad: "))
        {
            _languageHandler.kitchen.stirSalad = line.Substring(19, line.Length - 19);
        }
        else if (line.StartsWith("kitchen.fishPlate: "))
        {
            _languageHandler.kitchen.fishPlate = line.Substring(19, line.Length - 19);
        }

        else if (line.StartsWith("kitchen.choiceBurger: "))
        {
            _languageHandler.kitchen.choiceBurger = line.Substring(22, line.Length - 22);
        }
        else if (line.StartsWith("kitchen.choiceChili: "))
        {
            _languageHandler.kitchen.choiceChili = line.Substring(21, line.Length - 21);
        }
        else if (line.StartsWith("kitchen.choiceFish: "))
        {
            _languageHandler.kitchen.choiceFish = line.Substring(20, line.Length - 20);
        }
        else if (line.StartsWith("kitchen.fryFries: "))
        {
            _languageHandler.kitchen.fryFries = line.Substring(18, line.Length-18);
        }
    }

    private void ParseLineMisc(string line)
    {
        if (line.StartsWith("persUI.restart: "))
        {
            _languageHandler.persUI.restart = line.Substring(16, line.Length - 16);
        }
        else if (line.StartsWith("persUI.quit: "))
        {
            _languageHandler.persUI.quit = line.Substring(13, line.Length - 13);
        }
        else if (line.StartsWith("persUI.yes: "))
        {
            _languageHandler.persUI.yes = line.Substring(12, line.Length - 12);
        }
        else if (line.StartsWith("persUI.no: "))
        {
            _languageHandler.persUI.no = line.Substring(11, line.Length - 11);
        }
        else if (line.StartsWith("timeout.reminder: "))
        {
            _languageHandler.timeout.reminder = line.Substring(18, line.Length - 18);
        }
        else if (line.StartsWith("timeout.yes: "))
        {
            _languageHandler.timeout.reminder = line.Substring(13, line.Length - 13);
        }
        else if (line.StartsWith("misc.score: "))
        {
            _languageHandler.misc.score = line.Substring(12, line.Length-13);
        }
        else if(line.StartsWith("misc.lossText: "))
        {
            _languageHandler.misc.lossText = line.Substring(15, line.Length-15);
        }
    }
}
