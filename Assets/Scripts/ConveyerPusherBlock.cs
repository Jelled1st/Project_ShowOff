using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConveyerPusherBlock : MonoBehaviour
{
    private static readonly Vector3 InitScale = new Vector3(0.10f, 0.3f, 0.75f);

    private ConveyerBelt _conveyerBelt;
    private GameObject _lastWayPoint;
    private GameObject _currentWayPoint;
    private int _lastWayPointIndex;
    private int _currentWayPointIndex;

    private bool _initCalled = false;

    public void Init(string name, ConveyerBelt conveyerBelt, Transform parent, Vector3 position)
    {
        _conveyerBelt = conveyerBelt;

        this.gameObject.name = name;
        this.transform.SetParent(parent);
        BoxCollider collider;
        if (!this.gameObject.TryGetComponent<BoxCollider>(out collider))
        {
            collider = this.gameObject.AddComponent<BoxCollider>();
        }

        this.gameObject.transform.localScale = InitScale;
        this.gameObject.transform.localPosition = position;

        GameObject visualizer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualizer.transform.SetParent(this.transform);
        visualizer.transform.localPosition = new Vector3(0, 0, 0);
        visualizer.transform.localScale = new Vector3(1, 1, 1);

        _initCalled = true;
    }

    void Start()
    {
        if (!_initCalled) Debug.LogError("ConveyerPusherBlock not initialized, call Init() before Start()");
    }

    // Update is called once per frame
    void Update()
    {
        // if there is no waypoint to move to, return
        if (_currentWayPoint == null) return;

        if (this.transform.position == _currentWayPoint.transform.position)
        {
            SetCurrentWayPoint(null, -1, 0);
        }
    }

    #region waypoint getters and setters

    public GameObject GetLastWayPoint()
    {
        return _lastWayPoint;
    }

    public int GetLastWayPointIndex()
    {
        return _lastWayPointIndex;
    }

    public GameObject GetCurrentWayPoint()
    {
        return _currentWayPoint;
    }

    public int GetCurrentWayPointIndex()
    {
        return _currentWayPointIndex;
    }

    public void SetCurrentWayPoint(GameObject wayPoint, int index, float time)
    {
        _lastWayPoint = _currentWayPoint;
        _lastWayPointIndex = _currentWayPointIndex;

        _currentWayPoint = wayPoint;
        _currentWayPointIndex = index;

        if(_currentWayPoint != null)
        {
            this.transform.DOKill();
            this.transform.DOLocalMove(wayPoint.transform.position, time);
            this.transform.DOLocalRotate(wayPoint.transform.rotation.eulerAngles, time);
        }
    }

    #endregion
}
