using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmGameHandler : MonoBehaviour, IControlsObserver, IFarmPlotObserver
{
    [SerializeField] GameObject _swarmPrefab;
    private TouchController _touchController;

    // Start is called before the first frame update
    void Start()
    {
        GameObject controller = GameObject.FindGameObjectWithTag("Controller");
        if (controller == null) Debug.LogError("No controller found!");
        else
        {
            _touchController = controller.GetComponent<TouchController>();
            Subscribe(_touchController);
        }

        GameObject[] farmPlots = GameObject.FindGameObjectsWithTag("FarmPlot");
        for(int i = 0; i < farmPlots.Length; ++i)
        {
            Subscribe(farmPlots[i].GetComponent<FarmPlot>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick(ControllerHitInfo hitInfo)
    {
    }

    public void OnPress(ControllerHitInfo hitInfo)
    {
        FarmPlot plot;
        if (hitInfo.gameObject.TryGetComponent<FarmPlot>(out plot))
        {
        }
    }

    public void OnHold(float holdTime, ControllerHitInfo hitInfo)
    {
    }

    public void OnHoldRelease(float timeHeld, IControllable released)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint, ControllerHitInfo hitInfo)
    {
    }

    public void OnDrag(Vector3 position, IControllable dragged)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable dragged, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position, IControllable dragged)
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

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        if(state == FarmPlot.State.Planted)
        {
            GameObject swarmGO = Instantiate(_swarmPrefab);
            Swarm swarm = swarmGO.GetComponent<Swarm>();
            Vector3 randomPos = GetRandomSwarmPosition();
            swarm.Init(plot, randomPos + plot.transform.position);
        }
    }

    private Vector3 GetRandomSwarmPosition()
    {
        Vector3 random = new Vector3();
        random.x = Random.Range(-5, 5);
        random.y = 1;
        random.z = Random.Range(-5, 5);
        return random;
    }

    public void OnPlotHarvest(FarmPlot plot)
    {
    }
}
