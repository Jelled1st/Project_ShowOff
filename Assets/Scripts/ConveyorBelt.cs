using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private GameObject _conveyerBelt;
    [SerializeField] private GameObject _pusherBlockPrefab;
    [SerializeField] private GameObject _wayPoints;
    [SerializeField] private uint _sampleSize = 5;
    [Tooltip("Speed is how many second it takes to travel 1 unit")][SerializeField] private float _speed = 2f;
    [SerializeField] private bool _canTurn = true;
    [Tooltip("Turn interval in degrees")] [SerializeField] private uint _turnInterval = 90;

    private ConveyorPusherBlock[] _pusherBlocks;
    private bool _isTurning;
    private Tween _rotateTween;

    // Start is called before the first frame update
    void Start()
    {
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
        else if(_isTurning)
        {
            _isTurning = false;
            InitPusherBlocks();
        }
        for (int i = 0; i < _sampleSize; ++i)
        {
            if (_pusherBlocks[i] == null) continue;
            if(_pusherBlocks[i].GetCurrentWayPoint() == null)
            {
                int wayPointIndex = _pusherBlocks[i].GetLastWayPointIndex();
                if (wayPointIndex < _wayPoints.transform.childCount - 1)
                {
                    float wayPointDistance = GetSpaceBetweenWayPoints(wayPointIndex, ++wayPointIndex).magnitude;
                    _pusherBlocks[i].SetCurrentWayPoint(_wayPoints.transform.GetChild(wayPointIndex).gameObject, wayPointIndex, _speed * wayPointDistance);
                }
                else
                {
                    // reset to start
                    wayPointIndex = 1; // waypoint[0] is start point 
                    float wayPointDistance = GetSpaceBetweenWayPoints(0, 1).magnitude;
                    Transform wayPointTransform = _wayPoints.transform.GetChild(0);
                    _pusherBlocks[i].transform.SetPositionAndRotation(wayPointTransform.position, wayPointTransform.rotation);
                    _pusherBlocks[i].SetCurrentWayPoint(_wayPoints.transform.GetChild(wayPointIndex).gameObject, wayPointIndex, _speed * wayPointDistance);
                }
            }
        }
    }

    private Vector3 GetSpaceBetweenWayPoints(int wayPoint1, int wayPoint2)
    {
        Vector3 distance = _wayPoints.transform.GetChild(wayPoint2).position - _wayPoints.transform.GetChild(wayPoint1).position;
        return distance;
    }

    private Quaternion GetRotationBetweenWayPoints(int wayPoint1, int wayPoint2)
    {
        Quaternion rotation = Quaternion.FromToRotation(_wayPoints.transform.GetChild(wayPoint1).forward, _wayPoints.transform.GetChild(wayPoint2).forward);
        return rotation;
    }

    public void Turn()
    {
        if (_canTurn && (_rotateTween==null || !_rotateTween.IsPlaying()))
        {
            _rotateTween = this.gameObject.transform.DORotate(this.gameObject.transform.rotation.eulerAngles + new Vector3(0, _turnInterval, 0), 0.2f);
            _isTurning = true;
        }
    }

    private void InitPusherBlocks()
    {
        if(_pusherBlocks != null) for(int i = 0; i < _sampleSize; ++i)
        {
                Destroy(_pusherBlocks[i].transform.gameObject);
        }

        _pusherBlocks = new ConveyorPusherBlock[_sampleSize];
        //calculate the emptyspace up until the 
        for (int i = 0; i < _sampleSize; ++i)
        {
            GameObject pusherBlock = Instantiate(_pusherBlockPrefab);
            _pusherBlocks[i] = pusherBlock.GetComponent<ConveyorPusherBlock>();

            // calculates in between which waypoints the current pusherblock is and the distance to the next
            // gives a number like 1.6f meaning moving towards waypoint 2 and is 6/10th of the way there
            float inbetweenWayPoints = ((_wayPoints.transform.childCount - 1) / (float)(_sampleSize)) * i;
            //this calculates the waypoint where the current pusherblock will start
            int startWayPointIndex = Mathf.FloorToInt(inbetweenWayPoints);
            float inbetween = inbetweenWayPoints - (float)Math.Truncate((double)(inbetweenWayPoints));

            Transform startWayPointTransform = _wayPoints.transform.GetChild(startWayPointIndex);

            Vector3 beforeWayPointPosition = startWayPointTransform.transform.position;
            Vector3 distanceBetweenWayPoints = GetSpaceBetweenWayPoints(startWayPointIndex, startWayPointIndex + 1);
            Quaternion beforeWayPointRotation = startWayPointTransform.rotation;
            Quaternion rotationBetweenWayPoints = GetRotationBetweenWayPoints(startWayPointIndex, startWayPointIndex + 1);

            Quaternion added = beforeWayPointRotation * rotationBetweenWayPoints;
            Quaternion spawnRotation = Quaternion.Lerp(beforeWayPointRotation, added, inbetween);

            _pusherBlocks[i].Init("Pusher_Block_" + i, this, this.gameObject.transform, beforeWayPointPosition + distanceBetweenWayPoints * inbetween, spawnRotation);
            float wayPointDistance = (distanceBetweenWayPoints * (1 - inbetween)).magnitude;
            _pusherBlocks[i].SetCurrentWayPoint(_wayPoints.transform.GetChild(startWayPointIndex + 1).gameObject, startWayPointIndex + 1, _speed * wayPointDistance);
        }
    }
}
