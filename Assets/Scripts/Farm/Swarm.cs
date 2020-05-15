using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour, IFarmPlotObserver
{
    [SerializeField] private GameObject _swarmUnitPrefab;
    [Tooltip("SwarmSize, x is minimum size, y is maximum size")]
    [SerializeField] private Vector2Int _swarmSize = new Vector2Int(1, 3);
    [SerializeField] private float _spawnRange = 3.0f;
    [SerializeField] private float _speed = 0.5f;
    private FarmPlot _farmPlot;
    private List<GameObject> _swarmUnits;
    private bool _initCalled = false;
    private bool _continueSpawning = true;
    private List<float> _angles = new List<float>();
    Vector3 _destination;

    public void Init(FarmPlot plot)
    {
        _swarmUnits = new List<GameObject>();
        _farmPlot = plot;
        Subscribe(_farmPlot);
        initAngleList();
        _destination = _farmPlot.transform.position + new Vector3(0, 0.5f, 0);
        SpawnUnit();
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
        if(_continueSpawning)
        {

        }
        for (int i = 0; i < _swarmUnits.Count; ++i)
        {
            Vector3 diff = _destination - _swarmUnits[i].transform.position;
            _swarmUnits[i].transform.localPosition += diff.normalized * _speed * Time.deltaTime;
        }
    }

    private void SpawnUnit()
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
            Vector3 positionWithoutRotation = GetRandomPosition();
            Vector3 spawnPosition = angleRotation * positionWithoutRotation;
            unit.GetComponent<SwarmUnit>().Init(this);
            unit.transform.localPosition = spawnPosition;
            unit.transform.SetParent(this.transform);
            unit.transform.LookAt(_destination);

            --spawnCount;
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 random = new Vector2();
        random.x = UnityEngine.Random.Range(1, 2);
        random.y = 1.0f;
        random.z = UnityEngine.Random.Range(1, 2);
        if (UnityEngine.Random.Range(-1, 1) < 0) random.x *= -1;
        if (UnityEngine.Random.Range(-1, 1) < 0) random.z *= -1;
        return random * _spawnRange;
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
        Debug.Log("It ought to be removed");
        RemoveUnit(unit);
    }

    private void RemoveUnit(SwarmUnit unit)
    {
        _swarmUnits.Remove(unit.gameObject);
        Destroy(unit.gameObject);
        if (_swarmUnits.Count == 0) Destroy(this.gameObject);
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        _continueSpawning = false;
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
}
