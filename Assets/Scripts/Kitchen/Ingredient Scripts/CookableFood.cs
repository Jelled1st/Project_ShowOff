using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CookableFood : MonoBehaviour, IControllable, IIngredient
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private float _timeToCook = 2.0f;
    [HideInInspector] public CookingPan cookingPan;
    [SerializeField] List<CookableFood> _completeCookingWith;
    [SerializeField] List<CookableFood> _canOnlyBeAddedWithFood;
    private float _cookedTime = 0.0f;

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

    public void Cook(float modifier = 1.0f)
    {
        _cookedTime += Time.deltaTime *  modifier;
    }

    public bool IsCooked(bool withSideIngredients = false)
    {
        if (!withSideIngredients) return _cookedTime >= _timeToCook;
        else
        {
            if (_cookedTime < _timeToCook) return false;
            bool allDone = true;
            for(int i = 0; i < _completeCookingWith.Count; ++i)
            {
                if(!_completeCookingWith[i].IsCooked(true))
                {
                    allDone = false;
                    break;
                }
            }
            if (allDone) return true;
        }
        return false;
    }

    public List<CookableFood> GetRequiredHeadIngredients()
    {
        return _canOnlyBeAddedWithFood;
    }


    #region IIngredient
    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return IsCooked(true);
    }

    public void AddedToDish()
    {
        Destroy(this.gameObject);
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public GameObject GetDishMesh()
    {
        if (ReadyForDish())
        {
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<CookableFood>());
            Destroy(copy.GetComponent<Collider>());
            Renderer[] renderers = copy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }
            return copy;
        }
        return null;
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
        if (!(droppedOn is CuttingBoard))
        {
            cookingPan?.RemoveFood(this);
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<CookableFood>());
        Destroy(copy.GetComponent<Collider>());
        copy.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return copy;
    }
    #endregion
}
