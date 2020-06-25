using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlaneIngredient : MonoBehaviour, IIngredient, IControllable
{
    [SerializeField]
    private IngredientType _ingredientType;

    [SerializeField]
    private float _ingredientHeight;

    private void Awake()
    {
        gameObject.tag = "Ingredient";
    }

    #region IIngredient

    public void AddedToDish()
    {
        Destroy(gameObject);
    }

    public GameObject GetDishMesh()
    {
        var copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<PlaneIngredient>());
        Destroy(copy.GetComponent<Collider>());
        var renderers = copy.GetComponentsInChildren<Renderer>();
        for (var i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = true;
        }

        return copy;
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return true;
    }

    #endregion

    #region IControllable

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
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
    }

    public GameObject GetDragCopy()
    {
        var copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<PlaneIngredient>());
        Destroy(copy.GetComponent<Collider>());
        return copy;
    }

    #endregion


    public void Register(IObserver observer)
    {
    }

    public void UnRegister(IObserver observer)
    {
    }

    public void Notify(AObserverEvent observerEvent)
    {
        throw new System.NotImplementedException("Notify on PlaneIngredient is not implemented");
    }
}