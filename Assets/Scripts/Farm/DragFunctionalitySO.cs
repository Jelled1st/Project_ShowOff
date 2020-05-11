using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DragFunctionality", menuName = "ScriptableObjects/DragFunctionalitySO", order = 1)]
public class DragFunctionalitySO : ScriptableObject
{
    enum Functionalities
    {
        Plant,
        Water,
    }

    [SerializeField] private string _iconFile;
    [SerializeField] Functionalities _functionality;

    void Awake()
    {
    }

    public string GetIconFile()
    {
        return _iconFile;
    }

    public void Execute(FarmPlot plot)
    {
        if(_functionality == Functionalities.Plant)
        {
            plot.Plant();
        }
        else if(_functionality == Functionalities.Water)
        {
            plot.Water();
        }
    }
}
