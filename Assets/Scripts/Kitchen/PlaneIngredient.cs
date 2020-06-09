using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlaneIngredient : MonoBehaviour, IIngredient, IControllable
{
    [SerializeField] IngredientType _ingredientType;
    [SerializeField] float _ingredientHeight;
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
    }

    public GameObject GetDishMesh()
    {
        GameObject copy = GameObject.Instantiate(this.gameObject);
        Destroy(copy.GetComponent<PlaneIngredient>());
        Destroy(copy.GetComponent<Collider>());
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
}
