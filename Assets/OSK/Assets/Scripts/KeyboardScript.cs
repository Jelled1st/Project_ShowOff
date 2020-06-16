using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardScript : MonoBehaviour
{
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

    public void CloseAllLayouts()
    {
        RusLayoutSml.SetActive(false);
        RusLayoutBig.SetActive(false);
        EngLayoutSml.SetActive(false);
        EngLayoutBig.SetActive(false);
        SymbLayout.SetActive(false);
    }

    public void ShowLayout(GameObject SetLayout)
    {
        CloseAllLayouts();
        SetLayout.SetActive(true);
    }
}