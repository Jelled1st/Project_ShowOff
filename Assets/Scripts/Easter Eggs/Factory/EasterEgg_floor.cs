using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg_floor : MonoBehaviour
{
    [SerializeField]
    private Material _mat;

    private Color _colorBase;

    [SerializeField]
    private Color _color1;

    [SerializeField]
    private Color _color2;

    [SerializeField]
    private Color _color3;

    private int _colorCounter = 2;

    private void Start()
    {
        _colorBase = _mat.GetColor("_BaseColor");
    }

    private void OnMouseDown()
    {
        if (_colorCounter == 1)
        {
            _mat.DOColor(_colorBase, "_BaseColor", 1f);
        }

        if (_colorCounter == 2)
        {
            _mat.DOColor(_color1, "_BaseColor", 1f);
        }

        if (_colorCounter == 3)
        {
            _mat.DOColor(_color2, "_BaseColor", 1f);
        }

        if (_colorCounter == 4)
        {
            _mat.DOColor(_color3, "_BaseColor", 1f);
            _colorCounter = 0;
        }

        _colorCounter++;
    }

    private void OnDestroy()
    {
        _mat.SetColor("_BaseColor", _colorBase);
    }
}