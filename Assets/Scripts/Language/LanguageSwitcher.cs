using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SwitchToLanguagePack(LanguagePack languagePack)
    {
        LanguageHandler.instance.SwitchToLanguagePack(languagePack);
    }
}