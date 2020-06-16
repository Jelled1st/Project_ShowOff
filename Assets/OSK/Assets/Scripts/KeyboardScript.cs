using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardScript : MonoBehaviour
{
    public const char EnterChar = ' ';
    
    public static event Action<char> KeyPressed = delegate { };
    public static event Action BackspacePressed = delegate { };

    public InputField TextField;
    public GameObject RusLayoutSml, RusLayoutBig, EngLayoutSml, EngLayoutBig, SymbLayout;

    public void alphabetFunction(string alphabet)
    {
        // TextField.text = TextField.text + alphabet;
        KeyPressed(alphabet[0]);
    }

    public void BackSpace()
    {
        // if (TextField.text.Length > 0) TextField.text = TextField.text.Remove(TextField.text.Length - 1);
        BackspacePressed();
    }

    private void CloseAllLayouts()
    {
        RusLayoutSml.SetActive(false);
        RusLayoutBig.SetActive(false);
        EngLayoutSml.SetActive(false);
        EngLayoutBig.SetActive(false);
        SymbLayout.SetActive(false);
    }

    private void ShowLayout(GameObject SetLayout)
    {
        CloseAllLayouts();
        SetLayout.SetActive(true);
    }
}