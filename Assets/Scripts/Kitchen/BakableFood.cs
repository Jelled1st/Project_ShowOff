using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BakableFood : MonoBehaviour, IControllable, IIngredient
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private float _timeToBake;
    [SerializeField] private float _timeTillBurned = 10.0f;
    [SerializeField] private int _flipHeight = 10;
    [SerializeField] private ParticleSystem _smokeParticles;
    private float[] _bakedTimes = new float[2];
    private bool[] _sideIsBurned = new bool[2];
    private int _currentFace = 0;
    private bool _isBaking = false;
    private bool _wasBaking = false;
    [HideInInspector] public FryingPan fryingPan;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.MainModule main = _smokeParticles.main;
        main.playOnAwake = false;
        _smokeParticles.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBaking)
        {
            if (_bakedTimes[_currentFace] > _timeToBake)
            {
                _smokeParticles.Play();
            }
            else
            {
                _smokeParticles.Pause();
                _smokeParticles.Clear();
            }
            if(_bakedTimes[_currentFace] > _timeTillBurned)
            {
                if (!_sideIsBurned[_currentFace])
                {
                    ParticleSystem.MainModule main = _smokeParticles.main;
                    main.startColor = new Color(0.6f, 0.6f, 0.6f, 1.0f);
                    main.simulationSpeed = 1.8f;
                    _sideIsBurned[_currentFace] = true;
                }
            }
            else
            {
                ParticleSystem.MainModule main = _smokeParticles.main;
                main.startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                main.simulationSpeed = 1.0f;
            }
        }
        _wasBaking = _isBaking;
        _isBaking = false;
    }

    //called by the thing that does the baking
    public void Bake()
    {
        _isBaking = true;
        _bakedTimes[_currentFace] += Time.deltaTime;
        
    }

    public void Flip()
    {
        if ((_isBaking || _wasBaking) && !DOTween.IsTweening(this.transform))
        {
            _currentFace = (_currentFace+1) % 2;
            this.transform.DOPunchPosition(new Vector3(0, _ingredientHeight * _flipHeight, 0), 0.7f, 0);
            this.transform.DORotate(new Vector3(180, 0, 0), 0.4f, RotateMode.WorldAxisAdd);
            _smokeParticles.transform.rotation = Quaternion.identity;
        }
    }

    #region IIngredient
    public bool ReadyForDish()
    {
        return _bakedTimes[0] > _timeToBake && _bakedTimes[1] > _timeToBake;
    }

    public void AddedToDish()
    {
        fryingPan.RemoveFood(this);
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
            Destroy(copy.GetComponent<BakableFood>());
            Destroy(copy.GetComponent<Collider>());
            Destroy(copy.GetComponentInChildren<ParticleSystem>());
            return copy;
        }
        else return null;
    }

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    #endregion

    #region IControllable
    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<BakableFood>());
        Destroy(copy.GetComponent<Collider>());
        Destroy(copy.GetComponentInChildren<ParticleSystem>());
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
        Flip();
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }
    #endregion
}
