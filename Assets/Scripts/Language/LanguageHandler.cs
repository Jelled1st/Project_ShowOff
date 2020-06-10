using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageHandler : MonoBehaviour
{
    [SerializeField] LanguagePack _languagePack;

    public static LanguageHandler instance = null;

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
        if(field.StartsWith("main"))
        {
            if (field.StartsWith("main.start"))
            { 
                return main.start;
            }
            else if (field.StartsWith("main.exit"))
            {
                return main.exit;
            }
        }
        return "";
    }
}
