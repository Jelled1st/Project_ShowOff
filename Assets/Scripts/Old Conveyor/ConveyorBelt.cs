using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    private GameObject _conveyerBelt;

    [SerializeField]
    private GameObject _pusherBlockPrefab;

    [SerializeField]
    private GameObject _waypointsParent;

    [SerializeField]
    private uint _sampleSize = 5;

    [Tooltip("Speed is how many second it takes to travel 1 unit")]
    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private bool _canTurn = true;

    [Tooltip("Turn interval in degrees")]
    [SerializeField]
    private uint _turnInterval = 90;

    [SerializeField]
    private bool _inReverse = false;

    private ConveyorPusherBlock[] _pusherBlocks;
    private bool _isTurning;
    private Tween _rotateTween;
    private List<GameObject> _wayPoints;

    // Start is called before the first frame update
    private void Start()
    {
        _wayPoints = new List<GameObject>();
        if (_inReverse)
        {
            for (var i = _waypointsParent.transform.childCount - 1; i >= 0; --i)
            {
                _wayPoints.Add(_waypointsParent.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            for (var i = 0; i < _waypointsParent.transform.childCount; ++i)
            {
                _wayPoints.Add(_waypointsParent.transform.GetChild(i).gameObject);
            }
        }

        InitPusherBlocks();
        gameObject.tag = "ConveyorBelt";
    }

    // Update is called once per frame
    private void Update()
    {
        if (DOTween.IsTweening(gameObject.transform))
        {
            InitPusherBlocks();
        }
        else if (_isTurning)
        {
            _isTurning = false;
            InitPusherBlocks();
        }

        for (var i = 0; i < _sampleSize; ++i)
        {
            if (_pusherBlocks[i].GetCurrentWayPoint() == null)
            {
                var wayPointIndex = _pusherBlocks[i].GetLastWayPointIndex();
                if (wayPointIndex < _wayPoints.Count - 1)
                {
                    var wayPointDistance = GetSpaceBetweenWayPoints(wayPointIndex, ++wayPointIndex).magnitude;
                    _pusherBlocks[i].SetCurrentWayPoint(_wayPoints[wayPointIndex], wayPointIndex,
                        _speed * wayPointDistance);
                }
                else
                {
                    Debug.Log("Resetting");
                    // reset to start
                    wayPointIndex = 1; // waypoint[0] is start point 
                    var wayPointDistance = GetSpaceBetweenWayPoints(0, 1).magnitude;
                    var wayPointTransform = _wayPoints[0].transform;
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
        var distance = _wayPoints[wayPoint2].transform.position - _wayPoints[wayPoint1].transform.position;
        return distance;
    }

    private Quaternion GetRotationBetweenWayPoints(int wayPoint1, int wayPoint2)
    {
        var rotation = Quaternion.FromToRotation(_wayPoints[wayPoint1].transform.forward,
            _wayPoints[wayPoint2].transform.forward);
        return rotation;
    }

    public void Turn()
    {
        if (_canTurn && (_rotateTween == null || !_rotateTween.IsPlaying()))
        {
            _rotateTween =
                gameObject.transform.DORotate(
                    gameObject.transform.rotation.eulerAngles + new Vector3(0, _turnInterval, 0), 0.2f);
            _isTurning = true;
        }
    }

    private void InitPusherBlocks()
    {
        if (_pusherBlocks != null)
        {
            for (var i = 0; i < _sampleSize; ++i)
            {
                if (_pusherBlocks[i] != null) Destroy(_pusherBlocks[i].transform.gameObject);
            }
        }

        _pusherBlocks = new ConveyorPusherBlock[_sampleSize];
        //calculate the emptyspace up until the 
        for (var i = 0; i < _sampleSize; ++i)
        {
            var pusherBlock = Instantiate(_pusherBlockPrefab);
            _pusherBlocks[i] = pusherBlock.GetComponent<ConveyorPusherBlock>();

            // calculates in between which waypoints the current pusherblock is and the distance to the next
            // gives a number like 1.6f meaning moving towards waypoint 2 and is 6/10th of the way there
            var inbetweenWayPoints = (_wayPoints.Count - 1) / (float) _sampleSize * i;
            //this calculates the waypoint where the current pusherblock will start
            var startWayPointIndex = Mathf.FloorToInt(inbetweenWayPoints);
            var inbetween = inbetweenWayPoints - (float) Math.Truncate((double) inbetweenWayPoints);

            var startWayPointTransform = _wayPoints[startWayPointIndex].transform;

            var beforeWayPointPosition = startWayPointTransform.transform.position;
            var distanceBetweenWayPoints = GetSpaceBetweenWayPoints(startWayPointIndex, startWayPointIndex + 1);
            var beforeWayPointRotation = startWayPointTransform.rotation;
            var rotationBetweenWayPoints =
                GetRotationBetweenWayPoints(startWayPointIndex, startWayPointIndex + 1);

            var added = beforeWayPointRotation * rotationBetweenWayPoints;
            var spawnRotation = Quaternion.Lerp(beforeWayPointRotation, added, inbetween);

            _pusherBlocks[i].Init("Pusher_Block_" + i, this, gameObject.transform,
                beforeWayPointPosition + distanceBetweenWayPoints * inbetween, spawnRotation);
            var wayPointDistance = (distanceBetweenWayPoints * (1 - inbetween)).magnitude;
            _pusherBlocks[i].SetCurrentWayPoint(_wayPoints[startWayPointIndex + 1], startWayPointIndex + 1,
                _speed * wayPointDistance);
        }
    }
}