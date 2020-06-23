using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BakableFood : MonoBehaviour, IControllable, IIngredient, ISubject
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private float _timeToBake;
    [SerializeField] private float _startBurnTimeAfterBaking = 5.0f;
    [SerializeField] private float _timeTillBurned = 10.0f;
    [SerializeField] private float _flipHeight = 10;
    [SerializeField] private ParticleSystem _smokeParticles;
    [SerializeField] private GameObject _bakableObject;
    private float[] _bakedTimes = new float[2];
    private float[] _burntTimes = new float[2];
    private bool _sidesAreDone = false;
    private bool[] _sideIsDone = new bool[2] { false, false};
    private bool[] _sideIsBurned = new bool[2] { false, false};
    private bool[] _sideIsFullyBurned = new bool[2] { false, false};
    private int _currentFace = 0;
    private bool _isBaking = false;
    private bool _wasBaking = false;
    [HideInInspector] public FryingPan fryingPan;
    private Material[] _bakeMaterials = new Material[2];

    private List<IObserver> _observers = new List<IObserver>();

    void Awake()
    {
        this.gameObject.tag = "Ingredient";
    }

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.MainModule main = _smokeParticles.main;
        main.playOnAwake = false;
        _smokeParticles.Pause();

        Renderer rend = _bakableObject.GetComponentInChildren<Renderer>();
        for (int i = 0; i < 2; ++i)
        {
            _bakeMaterials[i] = rend.materials[i];
        }
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
            if(_bakedTimes[_currentFace] > StartBurnTime())
            {
                if (!_sideIsBurned[_currentFace])
                {
                    ParticleSystem.MainModule main = _smokeParticles.main;
                    main.startColor = new Color(0.6f, 0.6f, 0.6f, 1.0f);
                    main.simulationSpeed = 1.0f + Mathf.Min(_burntTimes[_currentFace] / _timeTillBurned, 1);
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

    private float StartBurnTime()
    {
        return _timeToBake + _startBurnTimeAfterBaking;
    }

    //called by the thing that does the baking
    public void Bake()
    {
        _isBaking = true;
        _bakedTimes[_currentFace] += Time.deltaTime;
        if(!_sideIsDone[_currentFace] && _bakedTimes[_currentFace] >= _timeToBake)
        {
            _sideIsDone[_currentFace] = true;
            Notify(new SideBakedEvent(this, _currentFace));
        }
        if(!_sidesAreDone && IsBaked())
        {
            _sidesAreDone = true;
            Notify(new IngredientDoneEvent(this));
        }
        if (_bakedTimes[_currentFace] >= StartBurnTime())
        {
            _burntTimes[_currentFace] += Time.deltaTime;
            if(_burntTimes[_currentFace] >= _timeTillBurned && !_sideIsFullyBurned[_currentFace])
            {
                _sideIsFullyBurned[_currentFace] = true;
                Scores.SubScore(300);
            }
            if (!_sideIsBurned[_currentFace])
            {
                _sideIsBurned[_currentFace] = true;
                Notify(new BakingStartBurnEvent(this));
            }
        }
        if (_bakeMaterials[_currentFace] != null)
        {
            _bakeMaterials[_currentFace].SetFloat("_MeatCooked", Mathf.Min(_bakedTimes[_currentFace] / _timeToBake, 1));

            if (_sideIsBurned[_currentFace])
            {
                _bakeMaterials[_currentFace].SetFloat("_MeatBurnt", Mathf.Min(_burntTimes[_currentFace] / _timeTillBurned, 1));
                _bakeMaterials[_currentFace].SetInt("_MeatBurningBool", 1);
            }
        }
    }

    public bool IsBaked()
    {
        return (_bakedTimes[0] >= _timeToBake && _bakedTimes[1] >= _timeToBake);
    }

    public bool IsBurnt()
    {
        return (_bakedTimes[0] >= _timeTillBurned && _bakedTimes[1] >= _timeTillBurned);
    }

    public void Flip()
    {
        if ((_isBaking || _wasBaking) && !DOTween.IsTweening(this.transform))
        {
            _currentFace = (_currentFace+1) % 2;
            this.transform.DOPunchPosition(new Vector3(0, _flipHeight, 0), 0.7f, 0);
            this.transform.DORotate(new Vector3(180, 0, 0), 0.4f, RotateMode.WorldAxisAdd);
            _smokeParticles.transform.rotation = Quaternion.identity;
            Notify(new BakingFlipEvent(this));
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
            Renderer[] renderers = copy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }
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

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}
