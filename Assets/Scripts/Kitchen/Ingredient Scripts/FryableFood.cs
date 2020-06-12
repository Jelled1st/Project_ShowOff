using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FryableFood : MonoBehaviour, IControllable, IIngredient
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private float _timeToFry;
    public FryFryer fryer;
    private float _friedTime = 0.0f; 

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

    public void Fry()
    {
        _friedTime += Time.deltaTime;
    }

    public float GetTimeFried()
    {
        return _friedTime;
    }

    public float GetFryTime()
    {
        return _timeToFry;
    }

    public bool IsFried()
    {
        return _friedTime >= _timeToFry;
    }

    #region IIngredient
    public void AddedToDish()
    {
        fryer.RemoveFood(this);
        Destroy(this.gameObject);
    }

    public GameObject GetDishMesh()
    {
        if (ReadyForDish())
        {
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<FryableFood>());
            Destroy(copy.GetComponent<Collider>());
            Renderer[] renderers = copy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }
            return copy;
        }
        else return null;
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
        if (_friedTime > _timeToFry)
        {
            return true;
        }
        else return false;
    }
    #endregion

    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<FryableFood>());
        Destroy(copy.GetComponent<Collider>());
        return copy;
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
