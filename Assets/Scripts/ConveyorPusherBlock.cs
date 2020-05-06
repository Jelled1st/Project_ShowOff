using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConveyorPusherBlock : MonoBehaviour
{
    private static readonly Vector3 InitScale = new Vector3(0.10f, 0.3f, 0.75f);

    private ConveyorBelt _conveyerBelt;
    private GameObject _lastWayPoint;
    private GameObject _currentWayPoint;
    private int _lastWayPointIndex;
    private int _currentWayPointIndex;

    private bool _initCalled = false;

    public void Init(string name, ConveyorBelt conveyerBelt, Transform parent, Vector3 position, Quaternion rotation)
    {
        _conveyerBelt = conveyerBelt;

        this.gameObject.name = name;
        this.transform.SetParent(parent);
        BoxCollider collider;
        if (!this.gameObject.TryGetComponent<BoxCollider>(out collider))
        {
            collider = this.gameObject.AddComponent<BoxCollider>();
        }

        GameObject visualizer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(visualizer.GetComponent<BoxCollider>());
        visualizer.transform.SetParent(this.transform);
        visualizer.transform.localPosition = new Vector3(0, 0, 0);
        visualizer.transform.localScale = new Vector3(1, 1, 1);

        this.gameObject.transform.position = position;
        this.gameObject.transform.Rotate(rotation.eulerAngles);
        this.gameObject.transform.localScale = InitScale;


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

        if (!DOTween.IsTweening(this.gameObject.transform))
        {
            SetCurrentWayPoint(null, -1, 0);
        }

        //if (this.transform.position == _currentWayPoint.transform.position)
        //{
        //    SetCurrentWayPoint(null, -1, 0);
        //}
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

        if (_currentWayPoint != null)
        {
            this.transform.DOKill();
            this.transform.DOMove(wayPoint.transform.position, time).SetEase(Ease.Linear);
            this.transform.DORotate(wayPoint.transform.rotation.eulerAngles, time);
        }
    }

    #endregion

    public ConveyorBelt GetConveyorBelt()
    {
        return _conveyerBelt;
    }

    private void OnDestroy()
    {
        
        this.transform.DOKill();
    }
}