using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageHandler : MonoBehaviour
{
    [SerializeField] LanguagePack _languagePack;

    public static LanguageHandler instance = null;
    private static List<LanguageText> _texts; 

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
    
    public struct FactorySceneText
    {
        public string washQuest;
        public string peelQuest;
        public string chopQuest;
        public string packageQuest;
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
    public FactorySceneText factory;
    public KitchenSceneText kitchen;
    public MiscText misc;


    void Awake()
    {
        if (instance != null) Destroy(this);
        _texts = new List<LanguageText>();
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        _languagePack.UnPack(this);
    }

    public void SwitchToLanguagePack(LanguagePack languagePack)
    {
        if (languagePack == _languagePack) return;
        _languagePack = languagePack;
        languagePack.UnPack(this);
        for(int i = 0; i < _texts.Count; ++i)
        {
            string text = GetTextForField(_texts[i].GetFieldName());
            _texts[i].SetText(text);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        _texts = new List<LanguageText>();
    }

    public string Register(LanguageText text, string field)
    {
        _texts.Add(text);
        return GetTextForField(field);
    }

    private string GetTextForField(string field)
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
        else if (field.StartsWith("factory.washQuest"))
        {
            return factory.washQuest;
        }
        else if (field.StartsWith("factory.peelQuest"))
        {
            return factory.peelQuest;
        }
        else if (field.StartsWith("factory.chopQuest"))
        {
            return factory.chopQuest;
        }
        else if (field.StartsWith("factory.packageQuest"))
        {
            return factory.packageQuest;
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
