using System.Collections.Generic;
using UnityEngine;

public class HarvestCollector : MonoBehaviour, IControllable
{
    [SerializeField]
    private List<GameObject> _collectedPotatoesPrefab;

    [SerializeField]
    private List<GameObject> _collectedPotatoPosition;

    private List<int> _potatoPositionsLeft;

    [SerializeField]
    private FarmGameHandler _gameHandler;

    private void Start()
    {
        _potatoPositionsLeft = new List<int>();
        for (var i = 0; i < _collectedPotatoPosition.Count; ++i)
        {
            _potatoPositionsLeft.Add(i);
        }
    }

    private void CollectPotato()
    {
        if (_potatoPositionsLeft.Count != 0)
        {
            var rand = Random.Range(0, _potatoPositionsLeft.Count);
            var index = _potatoPositionsLeft[rand];
            _potatoPositionsLeft.RemoveAt(rand);
            rand = Random.Range(0, _collectedPotatoesPrefab.Count);
            var potatoes = Instantiate(_collectedPotatoesPrefab[rand], _collectedPotatoPosition[index].transform);
            potatoes.transform.localPosition = new Vector3(0, 0, 0);
            potatoes.transform.localScale = new Vector3(1, 1, 1);
            if (_potatoPositionsLeft.Count == 0)
            {
                //last potato has been collected
                _gameHandler.LastPotatoCollected();
            }
        }
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if (dropped is FarmPlot)
        {
            var plot = (FarmPlot) dropped;
            if (plot.Harvest())
            {
                CollectPotato();
            }
        }
    }

    public GameObject GetDragCopy()
    {
        return null;
    }
}