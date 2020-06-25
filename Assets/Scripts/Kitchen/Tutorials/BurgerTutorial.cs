using UnityEngine;
using TMPro;

public class BurgerTutorial : KitchenSubTutorial
{
    [SerializeField]
    private TextMeshProUGUI _pattyToBurger;

    [SerializeField]
    private TextMeshProUGUI _finishedIngredientToBurger;

    [SerializeField]
    private Dish _burgerSideDish;

    private void Start()
    {
        _onBakingDone.AddListener(delegate { StrikeThroughText(_pattyToBurger); });
    }

    public override void IngredientDone(IIngredient ingredient)
    {
    }

    public override void DishComplete(Dish dish)
    {
        base.DishComplete(dish);
        if (dish == _burgerSideDish)
        {
            _finishedIngredientToBurger.fontStyle = FontStyles.Strikethrough;
        }
    }
}