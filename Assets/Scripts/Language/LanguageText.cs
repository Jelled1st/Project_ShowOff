using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshProUgui;
    [SerializeField] string _fieldName;

    // Start is called before the first frame update
    void Start()
    {
        string text = LanguageHandler.instance.RequestText(_fieldName);
        _textMeshProUgui.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
