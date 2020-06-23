using UnityEngine;
using TMPro;

public class BurgerTutorial : KitchenSubTutorial
{
    [SerializeField] TextMeshProUGUI _pattyToBurger;
    [SerializeField] TextMeshProUGUI _finishedIngredientToBurger;
    [SerializeField] Dish _burgerSideDish;

    // Start is called before the first frame update
    void Start()
    {
        _onBakingDone.AddListener(delegate { StrikeThroughText(_pattyToBurger); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void IngredientDone(IIngredient ingredient)
    {
        
    }

    public override void DishComplete(Dish dish)
    {
        base.DishComplete(dish);
        if(dish == _burgerSideDish)
        {
            _finishedIngredientToBurger.fontStyle = FontStyles.Strikethrough;
        }
    }
}
