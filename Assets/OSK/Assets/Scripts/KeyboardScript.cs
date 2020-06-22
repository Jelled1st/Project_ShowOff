using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardScript : MonoBehaviour
{
    public const char EnterChar = ' ';

    public static event Action<char> KeyPressed = delegate { };
    public static event Action BackspacePressed = delegate { };

    public GameObject RusLayoutSml, RusLayoutBig, EngLayoutSml, EngLayoutBig, SymbLayout;

    public bool InputEnabled { get; set; }


    private void OnEnable()
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public void alphabetFunction(string alphabet)
    {
        if (InputEnabled)
        {
            KeyPressed(alphabet[0]);
        }
    }

    public void BackSpace()
    {
        if (InputEnabled)
        {
            BackspacePressed();
        }
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