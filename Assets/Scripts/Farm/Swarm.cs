using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour, ISubject, IFarmPlotObserver, IGameHandlerObserver
{
    [SerializeField]
    private GameObject _swarmUnitPrefab;

    [Tooltip("SwarmSize, x is minimum size, y is maximum size")]
    [SerializeField]
    private Vector2Int _swarmSize = new Vector2Int(1, 3);

    [Tooltip("SwarmSize, x is minimum size, y is maximum size")]
    [SerializeField]
    private Vector2 _spawnRange = new Vector2(2.0f, 3.0f);

    [SerializeField]
    private int _angleAmount = 10;

    [SerializeField]
    private float _speed = 0.5f;

    [SerializeField]
    private float _fleeSpeed = 2.0f;

    [SerializeField]
    private float _spawnTime = 1.5f;

    [SerializeField]
    private Vector2 _outOfScreenMargin = new Vector2(100, 300);

    [Tooltip("Spawnchance in percentage")]
    [Range(0, 100)]
    [SerializeField]
    private float _spawnChance = 75;

    private FarmPlot _farmPlot;
    private List<GameObject> _swarmUnits;
    private bool _initCalled = false;
    private bool _continueSpawning = true;
    private bool _flee = false;
    private List<float> _angles = new List<float>();
    private float _timeSinceLastSpawn = 0.0f;
    private bool _ignoreSpawnTimer = true;
    private Vector3 _destination;
    private bool _paused;

    [SerializeField]
    private SFX soundEffectManager;

    private List<IObserver> _observers = new List<IObserver>();
    private static List<IObserver> _staticObservers;

    public void Init(FarmPlot plot)
    {
        var gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        _swarmUnits = new List<GameObject>();
        _farmPlot = plot;
        Subscribe(_farmPlot);
        InitAngleList();
        _destination = _farmPlot.transform.position + new Vector3(0, 0.5f, 0);

        NotifyStaticObservers(new SwarmSpawnEvent(this, this));
    }

    private void InitAngleList()
    {
        for (var i = 0; i < 360 / _angleAmount; ++i)
        {
            _angles.Add(i * _angleAmount);
        }
    }

    private void Start()
    {
        if (!_initCalled) Debug.LogWarning("Init not called before Start on Swarm");
        soundEffectManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SFX>();
    }

    private void Update()
    {
        if (_paused) return;
        for (var i = 0; i < _swarmUnits.Count; ++i)
        {
            var diff = _destination - _swarmUnits[i].transform.position;
            if (_flee)
            {
                _swarmUnits[i].transform.localPosition += diff.normalized * -_fleeSpeed * Time.deltaTime;
                if (Mathf.Abs(GetUnitAngleWithCamera(_swarmUnits[i])) > Camera.main.fieldOfView)
                    RemoveUnit(_swarmUnits[i].GetComponent<SwarmUnit>());
            }
            else
            {
                _swarmUnits[i].transform.localPosition += diff.normalized * _speed * Time.deltaTime;
            }
        }

        if (_continueSpawning)
        {
            _timeSinceLastSpawn += Time.deltaTime;
            if (_timeSinceLastSpawn >= _spawnTime || _ignoreSpawnTimer)
            {
                var rand = Random.Range(0.0f, 100.0f);
                if (rand <= _spawnChance)
                {
                    SpawnUnits();
                }
                else
                {
                    OnFailedSpawnChance();
                }
            }
        }
        else if (_swarmUnits.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnUnits()
    {
        var spawnCount = Random.Range(_swarmSize.x, _swarmSize.y + 1);
        while (spawnCount >= 0)
        {
            if (_angles.Count == 0) return;
            var angleIndex = Random.Range(0, _angles.Count);
            var angle = _angles[angleIndex];
            _angles.RemoveAt(angleIndex);
            var angleRotation = Quaternion.Euler(0, angle, 0);
            var spawnPosition = angleRotation * new Vector3(Random.Range(_spawnRange.x, _spawnRange.y), 1.0f, 0);
            var screenPosition = Camera.main.WorldToScreenPoint(spawnPosition);
            if (PointOutOfScreen(screenPosition))
            {
                continue;
            }

            var unit = Instantiate(_swarmUnitPrefab);
            _swarmUnits.Add(unit);
            var script = unit.GetComponent<SwarmUnit>();
            script.Init(this);
            unit.transform.SetParent(transform);
            unit.transform.localPosition = spawnPosition;
            unit.transform.LookAt(_destination);
            OnSpawnUnit(script);
            Notify(new SwarmBugSpawnEvent(this, script));

            --spawnCount;
        }
    }

    private bool PointOutOfScreen(Vector3 point)
    {
        if (point.x < _outOfScreenMargin.x || point.x > Screen.width - _outOfScreenMargin.x ||
            point.y < _outOfScreenMargin.y || point.y > Screen.height - _outOfScreenMargin.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float GetUnitAngleWithCamera(GameObject unit)
    {
        var toCam = unit.transform.position - Camera.main.transform.position;
        var angle = Vector3.Angle(toCam, Camera.main.transform.forward);
        return angle;
    }

    private void OnSpawnUnit(SwarmUnit unit)
    {
        //soundEffectManager.SoundSwarm();
        _timeSinceLastSpawn = 0.0f;
        _ignoreSpawnTimer = false;

        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is ISwarmObserver) (_observers[i] as ISwarmObserver).OnBugSpawn(unit);
        }
    }

    private void OnFailedSpawnChance()
    {
        _timeSinceLastSpawn = 0.0f;
        _ignoreSpawnTimer = false;

        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is ISwarmObserver) (_observers[i] as ISwarmObserver).OnBugspawnFail();
        }
    }

    private void OnFlee()
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is ISwarmObserver) (_observers[i] as ISwarmObserver).OnFlee();
        }

        for (var i = 0; i < _swarmUnits.Count; ++i)
        {
            _swarmUnits[i].transform.Rotate(new Vector3(0, 180, 0), Space.World);
        }
    }

    public void UnitReachedPlot(SwarmUnit unit, FarmPlot plot)
    {
        if (plot == _farmPlot)
        {
            _farmPlot.Decay();
            RemoveUnit(unit);
        }
    }

    public void UnitHit(SwarmUnit unit)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is ISwarmObserver) (_observers[i] as ISwarmObserver).OnBugKill(unit);
        }

        Notify(new SwarmBugKillEvent(unit, this));
        RemoveUnit(unit);
    }

    private void RemoveUnit(SwarmUnit unit)
    {
        _swarmUnits.Remove(unit.gameObject);

        //soundEffectManager.SoundSwarmStop();

        Destroy(unit.gameObject);
        if (_swarmUnits.Count == 0 && !_continueSpawning) Destroy(gameObject);
    }

    public void OnPlotStartStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        if (state != FarmPlot.State.Growing)
        {
            _continueSpawning = false;
            _flee = true;
            OnFlee();
        }

        if (state == FarmPlot.State.Healing)
        {
        }
    }

    public void OnPlotHarvest(FarmPlot plot)
    {
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    public void OnPause()
    {
        if (!_paused) _paused = true;
    }

    public void OnContinue()
    {
        if (_paused) _paused = false;
    }

    public void OnFinish()
    {
        if (!_paused) _paused = true;
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
    }

    #region ISubject

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public static void RegisterStatic(IObserver observer)
    {
        if (_staticObservers == null) _staticObservers = new List<IObserver>();
        _staticObservers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public static void UnRegisterStatic(IObserver observer)
    {
        _staticObservers?.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }

    private static void NotifyStaticObservers(AObserverEvent observerEvent)
    {
        if (_staticObservers == null) return;
        for (var i = 0; i < _staticObservers.Count; ++i)
        {
            _staticObservers[i].OnNotify(observerEvent);
        }
    }

    #endregion

    public void OnDestroy()
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i] is ISwarmObserver) (_observers[i] as ISwarmObserver).OnSwarmDestroy();
        }
    }
}