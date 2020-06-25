using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI _textMeshProUgui;

    [SerializeField]
    protected string _fieldName;

    private void Start()
    {
        if (_textMeshProUgui == null) TryGetComponent<TextMeshProUGUI>(out _textMeshProUgui);

        if (_textMeshProUgui == null) return;
        var text = LanguageHandler.instance.Register(this, _fieldName);
        _textMeshProUgui.text = text;
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