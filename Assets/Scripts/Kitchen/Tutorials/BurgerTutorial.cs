﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BurgerTutorial : KitchenSubTutorial
{
    [SerializeField] TextMeshProUGUI _pattyToBurger;
    [SerializeField] TextMeshProUGUI _finishedIngredientToBurger;

    // Start is called before the first frame update
    void Start()
    {
        _onBakingDone.AddListener(delegate { StrikeThroughText(_pattyToBurger); });
        _onIngredientToDish.AddListener(delegate { StrikeThroughText(_finishedIngredientToBurger); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void IngredientDone(IIngredient ingredient)
    {
        
    }
}
