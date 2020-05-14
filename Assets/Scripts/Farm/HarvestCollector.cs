using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestCollector : MonoBehaviour, IControllable
{
    [SerializeField] private List<GameObject> _collectedPotatoesPrefab;
    [SerializeField] private List<GameObject> _collectedPotatoPosition;
    List<int> _potatoPositionsLeft;

    // Start is called before the first frame update
    void Start()
    {
        _potatoPositionsLeft = new List<int>();
        for(int i = 0; i < _collectedPotatoPosition.Count; ++i)
        {
            _potatoPositionsLeft.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void CollectPotato()
    {
        if(_potatoPositionsLeft.Count != 0)
        {
            int rand = Random.Range(0, _potatoPositionsLeft.Count);
            int index = _potatoPositionsLeft[rand];
            _potatoPositionsLeft.RemoveAt(rand);
            rand = Random.Range(0, _collectedPotatoesPrefab.Count);
            GameObject potatoes = Instantiate(_collectedPotatoesPrefab[rand], _collectedPotatoPosition[index].transform);
            potatoes.transform.localPosition = new Vector3(0, 0, 0);
            potatoes.transform.localScale = new Vector3(1, 1, 1);
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
        if(dropped is FarmPlot)
        {
            FarmPlot plot = (FarmPlot)dropped;
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
