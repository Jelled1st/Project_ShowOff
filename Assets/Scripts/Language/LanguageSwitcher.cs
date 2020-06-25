using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public void SwitchToLanguagePack(LanguagePack languagePack)
    {
        LanguageHandler.instance.SwitchToLanguagePack(languagePack);
    }
}