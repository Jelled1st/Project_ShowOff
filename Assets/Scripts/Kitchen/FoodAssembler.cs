using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FoodAssembler : MonoBehaviour, IControllable
{
    [SerializeField] private GameObject _ingredientPosition;
    [SerializeField] private List<IngredientType> _requiredIngredients;
    [SerializeField] private List<IngredientType> _optionalIngredients;
    private List<IngredientType> _addedIngredients = new List<IngredientType>();
    private Dictionary<IngredientType, GameObject> _addedIngredientObjects = new Dictionary<IngredientType, GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool TryAddIngredient(IIngredient ingredient)
    {
        IngredientType type = ingredient.GetIngredientType();
        for (int i = 0; i < _requiredIngredients.Count; ++i)
        {
            if (type == _requiredIngredients[i])
            {
                //required ingredient
                _requiredIngredients.RemoveAt(i); //remove ingredient from the list
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight());
                return true;
            }
        }
        for (int i = 0; i < _optionalIngredients.Count; ++i)
        {
            if (type == _optionalIngredients[i])
            {
                //required ingredient
                _optionalIngredients.RemoveAt(i); //remove ingredient from the list
                AddIngredientMesh(type, ingredient.GetDishMesh(), ingredient.GetHeight());
                return true;
            }
        }
        return false;
    }

    private void AddIngredientMesh(IngredientType type, GameObject ingredientMesh, float ingredientHeight)
    {
        if (ingredientMesh == null) return;
        _addedIngredients.Add(type);
        GameObject ingredientGO = Instantiate(ingredientMesh);
        _addedIngredientObjects.Add(type, ingredientGO);
        ingredientGO.transform.SetParent(this.transform);
        ingredientGO.transform.position = _ingredientPosition.transform.position;
        ingredientGO.transform.rotation = _ingredientPosition.transform.rotation;
        _ingredientPosition.transform.position += new Vector3(0, ingredientHeight, 0);
    }

    public GameObject GetDragCopy()
    {
        return null;
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if(dropped is IIngredient)
        {
            IIngredient ingredient = dropped as IIngredient;
            if (ingredient.ReadyForDish()) if(TryAddIngredient(ingredient)) ingredient.AddedToDish();
        }
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnPress(Vector3 hitPoint)
    { 
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }

}
