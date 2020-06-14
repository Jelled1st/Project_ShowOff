using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private GameObject _conveyerBelt;
    [SerializeField] private GameObject _pusherBlockPrefab;
    [SerializeField] private GameObject _waypointsParent;
    [SerializeField] private uint _sampleSize = 5;

    [Tooltip("Speed is how many second it takes to travel 1 unit")] [SerializeField]
    private float _speed = 2f;

    [SerializeField] private bool _canTurn = true;

    [Tooltip("Turn interval in degrees")] [SerializeField]
    private uint _turnInterval = 90;

    [SerializeField] private bool _inReverse = false;

    private ConveyorPusherBlock[] _pusherBlocks;
    private bool _isTurning;
    private Tween _rotateTween;
    private List<GameObject> _wayPoints;

    // Start is called before the first frame update
    void Start()
    {
        _wayPoints = new List<GameObject>();
        if (_inReverse)
        {
            for (int i = _waypointsParent.transform.childCount - 1; i >= 0; --i)
            {
                _wayPoints.Add(_waypointsParent.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = 0; i < _waypointsParent.transform.childCount; ++i)
            {
                _wayPoints.Add(_waypointsParent.transform.GetChild(i).gameObject);
            }
        }

        InitPusherBlocks();
        this.gameObject.tag = "ConveyorBelt";
    }

    // Update is called once per frame
    void Update()
    {
        if (DOTween.IsTweening(this.gameObject.transform))
        {
            InitPusherBlocks();
        }
        else if (_isTurning)
        {
            _isTurning = false;
            InitPusherBlocks();
        }

        for (int i = 0; i < _sampleSize; ++i)
        {
            if (_pusherBlocks[i].GetCurrentWayPoint() == null)
            {
                int wayPointIndex = _pusherBlocks[i].GetLastWayPointIndex();
                if (wayPointIndex < _wayPoints.Count - 1)
                {
                    float wayPointDistance = GetSpaceBetweenWayPoints(wayPointIndex, ++wayPointIndex).magnitude;
                    _pusherBlocks[i].SetCurrentWayPoint(_wayPoints[wayPointIndex], wayPointIndex,
                        _speed * wayPointDistance);
                }
                else
                {
                    Debug.Log("Resetting");
                    // reset to start
                    wayPointIndex = 1; // waypoint[0] is start point 
                    float wayPointDistance = GetSpaceBetweenWayPoints(0, 1).magnitude;
                    Transform wayPointTransform = _wayPoints[0].transform;
                    _pusherBlocks[i].transform
                        .SetPositionAndRotation(wayPointTransform.position, wayPointTransform.rotation);
                    _pusherBlocks[i].SetCurrentWayPoint(_wayPoints[wayPointIndex], wayPointIndex,
                        _speed * wayPointDistance);
                }
            }
        }
    }

    private Vector3 GetSpaceBetweenWayPoints(int wayPoint1, int wayPoint2)
    {
        Vector3 distance = _wayPoints[wayPoint2].transform.position - _wayPoints[wayPoint1].transform.position;
        return distance;
    }

    private Quaternion GetRotationBetweenWayPoints(int wayPoint1, int wayPoint2)
    {
        Quaternion rotation = Quaternion.FromToRotation(_wayPoints[wayPoint1].transform.forward,
            _wayPoints[wayPoint2].transform.forward);
        return rotation;
    }

    public void Turn()
    {
        if (_canTurn && (_rotateTween == null || !_rotateTween.IsPlaying()))
        {
            _rotateTween =
                this.gameObject.transform.DORotate(
                    this.gameObject.transform.rotation.eulerAngles + new Vector3(0, _turnInterval, 0), 0.2f);
            _isTurning = true;
        }
    }

    private void InitPusherBlocks()
    {
        if (_pusherBlocks != null)
        {
            for (int i = 0; i < _sampleSize; ++i)
            {
                if (_pusherBlocks[i] != null) Destroy(_pusherBlocks[i].transform.gameObject);
            }
        }

        _pusherBlocks = new ConveyorPusherBlock[_sampleSize];
        //calculate the emptyspace up until the 
        for (int i = 0; i < _sampleSize; ++i)
        {
            GameObject pusherBlock = Instantiate(_pusherBlockPrefab);
            _pusherBlocks[i] = pusherBlock.GetComponent<ConveyorPusherBlock>();

            // calculates in between which waypoints the current pusherblock is and the distance to the next
            // gives a number like 1.6f meaning moving towards waypoint 2 and is 6/10th of the way there
            float inbetweenWayPoints = ((_wayPoints.Count - 1) / (float) (_sampleSize)) * i;
            //this calculates the waypoint where the current pusherblock will start
            int startWayPointIndex = Mathf.FloorToInt(inbetweenWayPoints);
            float inbetween = inbetweenWayPoints - (float) Math.Truncate((double) (inbetweenWayPoints));

            Transform startWayPointTransform = _wayPoints[startWayPointIndex].transform;

            Vector3 beforeWayPointPosition = startWayPointTransform.transform.position;
            Vector3 distanceBetweenWayPoints = GetSpaceBetweenWayPoints(startWayPointIndex, startWayPointIndex + 1);
            Quaternion beforeWayPointRotation = startWayPointTransform.rotation;
            Quaternion rotationBetweenWayPoints =
                GetRotationBetweenWayPoints(startWayPointIndex, startWayPointIndex + 1);

            Quaternion added = beforeWayPointRotation * rotationBetweenWayPoints;
            Quaternion spawnRotation = Quaternion.Lerp(beforeWayPointRotation, added, inbetween);

            _pusherBlocks[i].Init("Pusher_Block_" + i, this, this.gameObject.transform,
                beforeWayPointPosition + distanceBetweenWayPoints * inbetween, spawnRotation);
            float wayPointDistance = (distanceBetweenWayPoints * (1 - inbetween)).magnitude;
            _pusherBlocks[i].SetCurrentWayPoint(_wayPoints[startWayPointIndex + 1], startWayPointIndex + 1,
                _speed * wayPointDistance);
        }
    }
}