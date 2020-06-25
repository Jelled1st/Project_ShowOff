using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Image _backgroundColor;

    [SerializeField]
    private Image _fillColor;

    public void SetActive(bool active)
    {
        _slider.gameObject.SetActive(active);
    }

    //set procentage from 0-1
    public void SetPercentage(float percentage)
    {
        _slider.value = percentage;
    }

    public float GetPercentage()
    {
        return _slider.value;
    }

    public void SetBackgroundColor(Color color)
    {
        _backgroundColor.color = color;
    }

    public void SetFillColor(Color color)
    {
        _fillColor.color = color;
    }
}