using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    [SerializeField] private GameObject _conveyerBelt;
    [SerializeField] private GameObject _pusherBlockPrefab;
    [SerializeField] private GameObject _wayPoints;
    [SerializeField] private uint _sampleSize = 5;
    [Tooltip("Speed is how many second it takes to travel 1 unit")][SerializeField] private float _speed = 2f;

    private ConveyerPusherBlock[] _pusherBlocks;

    // Start is called before the first frame update
    void Start()
    {
        _pusherBlocks = new ConveyerPusherBlock[_sampleSize];
        //calculate the emptyspace up until the 
        Vector3 emptySpace = GetSpaceBetweenWayPoints(0, _wayPoints.transform.childCount - 1) / (_sampleSize);
        Vector3 spawnPosition = _wayPoints.transform.GetChild(0).transform.localPosition;
        for(int i = 0; i < _sampleSize; ++i)
        {
            GameObject pusherBlock = Instantiate(_pusherBlockPrefab);
            _pusherBlocks[i] = pusherBlock.GetComponent<ConveyerPusherBlock>();
            _pusherBlocks[i].Init("Pusher_Block_" + i, this, this.gameObject.transform, spawnPosition + (emptySpace * i));
            float wayPointDistance = (_wayPoints.transform.GetChild(1).position - (spawnPosition + (emptySpace * i))).magnitude;
            _pusherBlocks[i].SetCurrentWayPoint(_wayPoints.transform.GetChild(1).gameObject, 1, _speed * wayPointDistance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _sampleSize; ++i)
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
                    Debug.Log("Reset " + i);
                    // reset to start
                    wayPointIndex = 1; // waypoint[0] is start point 
                    float wayPointDistance = GetSpaceBetweenWayPoints(0, 1).magnitude;
                    Transform wayPointTransform = _wayPoints.transform.GetChild(0);
                    _pusherBlocks[i].transform.SetPositionAndRotation(wayPointTransform.localPosition, wayPointTransform.localRotation);
                    _pusherBlocks[i].SetCurrentWayPoint(_wayPoints.transform.GetChild(wayPointIndex).gameObject, wayPointIndex, _speed * wayPointDistance);
                }
            }
        }
    }

    private Vector3 GetSpaceBetweenWayPoints(int waypoint1, int waypoint2)
    {
        Vector3 distance = _wayPoints.transform.GetChild(waypoint2).transform.localPosition - _wayPoints.transform.GetChild(waypoint1).transform.localPosition;
        return distance;
    }
}
