using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageHandler : MonoBehaviour
{
    [SerializeField] LanguagePack _languagePack;

    public static LanguageHandler instance = null;

    #region text structs
    public struct MainSceneText
    {
        public string start;
        public string exit;
    }

    public struct FarmSceneText
    {
        public string mainQuest;
        public string shovelQuest;
        public string plantQuest;
        public string waterQuest;
    }

    public struct KitchenSceneText
    {
        public string fryFries;
        public string cookPatty;
    }

    public struct MiscText
    {
        public string score;
    }
    #endregion

    public MainSceneText main;
    public FarmSceneText farm;
    public KitchenSceneText kitchen;
    public MiscText misc;


    void Awake()
    {
        if (instance != null) Destroy(this.gameObject);
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        _languagePack.UnPack(this);
    }

    public string RequestText(string field)
    {
        if (field.StartsWith("main.start"))
        {
            return main.start;
        }
        else if (field.StartsWith("main.exit"))
        {
            return main.exit;
        }
        else if (field.StartsWith("farm.mainQuest"))
        {
            return farm.mainQuest;
        }
        else if (field.StartsWith("farm.shovelQuest"))
        {
            return farm.shovelQuest;
        }
        else if (field.StartsWith("farm.plantQuest"))
        {
            return farm.plantQuest;
        }
        else if (field.StartsWith("farm.waterQuest"))
        {
            return farm.waterQuest;
        }
        else if (field.StartsWith("kitchen.fryFries"))
        {
            return kitchen.fryFries;
        }
        else if (field.StartsWith("kitchen.cookPatty"))
        {
            return kitchen.cookPatty;
        }
        else if (field.StartsWith("misc.score"))
        {
            return misc.score;
        }
        return "";
    }
}
