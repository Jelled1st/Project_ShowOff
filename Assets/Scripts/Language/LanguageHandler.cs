using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageHandler : MonoBehaviour
{
    [SerializeField] LanguagePack _languagePack;

    public static LanguageHandler instance = null;

    public struct Main
    {
        public string start;
        public string exit;
    }

    public Main main;


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
