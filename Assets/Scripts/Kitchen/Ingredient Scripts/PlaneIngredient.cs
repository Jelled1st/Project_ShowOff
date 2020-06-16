using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlaneIngredient : MonoBehaviour, IIngredient, IControllable
{
    [SerializeField] IngredientType _ingredientType;
    [SerializeField] float _ingredientHeight;

    void Awake()
    {
        this.gameObject.tag = "Ingredient";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region IIngredient
    public void AddedToDish()
    {
        Destroy(this.gameObject);
    }

    public GameObject GetDishMesh()
    {
        GameObject copy = GameObject.Instantiate(this.gameObject);
        Destroy(copy.GetComponent<PlaneIngredient>());
        Destroy(copy.GetComponent<Collider>()); 
        Renderer[] renderers = copy.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
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
        GameObject copy = GameObject.Instantiate(this.gameObject);
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
