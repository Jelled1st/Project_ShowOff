using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour, IFarmPlotObserver, IGameHandlerObserver
{
    [SerializeField] private GameObject _swarmUnitPrefab;
    [Tooltip("SwarmSize, x is minimum size, y is maximum size")]
    [SerializeField] private Vector2Int _swarmSize = new Vector2Int(1, 3);
    [Tooltip("SwarmSize, x is minimum size, y is maximum size")]
    [SerializeField] private Vector2 _spawnRange = new Vector2(2.0f, 3.0f);
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _spawnTime = 1.5f;
    [Tooltip("Spawnchance in percentage")]
    [Range(0, 100)] [SerializeField] private float _spawnChance = 75;
    private FarmPlot _farmPlot;
    private List<GameObject> _swarmUnits;
    private bool _initCalled = false;
    private bool _continueSpawning = true;
    private List<float> _angles = new List<float>();
    private float _timeSinceLastSpawn = 0.0f;
    bool _ignoreSpawnTimer = true;
    Vector3 _destination;
    private static bool _paused;

    public void Init(FarmPlot plot)
    {
        GameObject gameHandler = GameObject.FindGameObjectWithTag("GameHandler");
        ISubject gameHandlerSubject;
        if (gameHandler.TryGetComponent<ISubject>(out gameHandlerSubject))
        {
            Subscribe(gameHandlerSubject);
        }

        _swarmUnits = new List<GameObject>();
        _farmPlot = plot;
        Subscribe(_farmPlot);
        initAngleList();
        _destination = _farmPlot.transform.position + new Vector3(0, 0.5f, 0);
    }

    private void initAngleList()
    {
        int angleAmount = 10;
        for(int i = 0; i < (360/angleAmount); ++i)
        {
            _angles.Add(i * angleAmount);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_initCalled) Debug.LogWarning("Init not called before Start on Swarm");
    }

    // Update is called once per frame
    void Update()
    {
        if (_paused) return;
        for (int i = 0; i < _swarmUnits.Count; ++i)
        {
            Vector3 diff = _destination - _swarmUnits[i].transform.position;
            _swarmUnits[i].transform.localPosition += diff.normalized * _speed * Time.deltaTime;
        }
        if (_continueSpawning)
        {
            _timeSinceLastSpawn += Time.deltaTime;
            if (_timeSinceLastSpawn >= _spawnTime || _ignoreSpawnTimer)
            {
                float rand = Random.Range(0.0f, 100.0f);
                if (rand <= _spawnChance)
                {
                    SpawnUnits();
                    OnSpawnUnit();
                }
                else OnFailedSpawnChance();
            }
        }
    }

    private void SpawnUnits()
    {
        if (_angles.Count == 0) return;
        int spawnCount = Random.Range(_swarmSize.x, _swarmSize.y+1);
        while(spawnCount >= 0)
        {
            int angleIndex = Random.Range(0, _angles.Count);
            float angle = _angles[angleIndex];
            _angles.RemoveAt(angleIndex);
            GameObject unit = Instantiate(_swarmUnitPrefab);
            _swarmUnits.Add(unit);
            Quaternion angleRotation = Quaternion.Euler(0, angle, 0);
            Vector3 spawnPosition = angleRotation * new Vector3(Random.Range(_spawnRange.x, _spawnRange.y), 1.0f, 0);
            unit.GetComponent<SwarmUnit>().Init(this);
            unit.transform.localPosition = spawnPosition;
            unit.transform.SetParent(this.transform);
            unit.transform.LookAt(_destination);

            --spawnCount;
        }
    }

    private void OnSpawnUnit()
    {
        _timeSinceLastSpawn = 0.0f;
        _ignoreSpawnTimer = false;
    }

    private void OnFailedSpawnChance()
    {
        _timeSinceLastSpawn = 0.0f;
        _ignoreSpawnTimer = false;
    }

    public void UnitReachedPlot(SwarmUnit unit, FarmPlot plot)
    {
        if(plot == _farmPlot)
        {
            _farmPlot.Decay();
            RemoveUnit(unit);
        }
    }

    public void UnitHit(SwarmUnit unit)
    {
        RemoveUnit(unit);
    }

    private void RemoveUnit(SwarmUnit unit)
    {
        _swarmUnits.Remove(unit.gameObject);
        Destroy(unit.gameObject);
        if (_swarmUnits.Count == 0 && !_continueSpawning) Destroy(this.gameObject);
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        if(state != FarmPlot.State.Growing) _continueSpawning = false;
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
}
