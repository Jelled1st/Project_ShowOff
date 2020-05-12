using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour
{
    [SerializeField] private GameObject _swarmUnitPrefab;
    [Tooltip("SwarmSize, x is minimum size, y is maximum size")]
    [SerializeField] private Vector2Int _swarmSize = new Vector2Int(2, 5);
    [SerializeField] private float _speed = 0.5f;
    private FarmPlot _farmPlot;
    private List<GameObject> _swarmUnits;
    private bool _initCalled = false;

    public void Init(FarmPlot destination, Vector3 startPosition)
    {
        int rand = Random.Range(_swarmSize.x, _swarmSize.y);
        _swarmUnits = new List<GameObject>();
        Vector3 plotPos = destination.transform.position;
        for (int i = 0; i < rand; ++i)
        {
            GameObject swarmUnit = Instantiate(_swarmUnitPrefab);
            _swarmUnits.Add(swarmUnit);
            swarmUnit.transform.SetParent(this.transform);
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            swarmUnit.transform.position = startPosition + offset;
            swarmUnit.transform.LookAt(plotPos);
            swarmUnit.GetComponent<SwarmUnit>().Init(this);
        }
        _farmPlot = destination;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_initCalled) Debug.LogWarning("Init not called before Start on Swarm");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 destination = _farmPlot.transform.position + new Vector3(0, 0.5f, 0);
        for (int i = 0; i < _swarmUnits.Count; ++i)
        {
            Vector3 diff = destination - _swarmUnits[i].transform.position;
            _swarmUnits[i].transform.localPosition += diff.normalized * _speed * Time.deltaTime;
        }
    }

    public void UnitEnterPlot(SwarmUnit unit, FarmPlot plot)
    {
        if(plot == _farmPlot)
        {
            _swarmUnits.Remove(unit.gameObject);
            Destroy(unit.gameObject);
        }
    }
}
