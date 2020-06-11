using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _textMeshProUgui;
    [SerializeField] protected string _fieldName;

    // Start is called before the first frame update
    void Start()
    {
        if (_textMeshProUgui == null) this.TryGetComponent<TextMeshProUGUI>(out _textMeshProUgui);

        if (_textMeshProUgui == null) return;
        string text = LanguageHandler.instance.Register(this, _fieldName);
        _textMeshProUgui.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetText(string text)
    {
        if (_textMeshProUgui == null) return;
        _textMeshProUgui.text = text;
    }

    public virtual string GetFieldName()
    {
        return _fieldName;
    }
}
