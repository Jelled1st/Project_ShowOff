using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmGameHandler : MonoBehaviour, IControlsObserver, IFarmPlotObserver
{
    [SerializeField] GameObject _swarmPrefab;
    private TouchController _touchController;
    [Header("SpawnStates and rates")]
    [SerializeField] List<FarmPlotSpawnStateRate> _stateSpawnRates;

    bool _debugLog = false;

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

        GameObject[] farmPlotsGOs = GameObject.FindGameObjectsWithTag("FarmPlot");
        //Set states before substribing
        FarmPlot[] farmPlots = new FarmPlot[farmPlotsGOs.Length];
        for(int i = 0; i < farmPlotsGOs.Length; ++i)
        {
            farmPlots[i] = farmPlotsGOs[i].GetComponent<FarmPlot>();
        }
        SetFarmPlotStates(farmPlots);
        for(int i = 0; i < farmPlots.Length; ++i)
        {
            Subscribe(farmPlots[i]);
        }
    }

    private void SetFarmPlotStates(FarmPlot[] farmPlots)
    {
        if(_stateSpawnRates == null || _stateSpawnRates.Count == 0)
        {
            for(int i = 0; i < farmPlots.Length; ++i)
            {
                farmPlots[i].SetStartState(FarmPlot.State.Rough);
            }
            return;
        }

        List<int> farmPlotsToBeSet = new List<int>();
        for (int i = 0; i < farmPlots.Length; ++i)
        {
            farmPlotsToBeSet.Add(i);
        }
        _stateSpawnRates.Sort();
        for (int i = 0; i < _stateSpawnRates.Count; ++i)
        {
            if (farmPlotsToBeSet.Count == 0) break;
            if(_debugLog) Debug.Log("Applying spawn rate " + _stateSpawnRates[i].state);
            List<int> chosenIndeces = GetAppliedStateSpawnRate(_stateSpawnRates[i], farmPlotsToBeSet);
            for(int j = 0; j < chosenIndeces.Count; ++j)
            {
                if(_debugLog) Debug.Log("Chosen index[" + j + "] = " + chosenIndeces[j]);
                farmPlots[chosenIndeces[j]].SetStartState(_stateSpawnRates[i].state);
            }
            for(int j = chosenIndeces.Count -1; j >= 0; --j)
            {
                farmPlotsToBeSet.Remove(chosenIndeces[j]);
            }
        }
    }

    //returns a list of chosen indecis by rng
    private List<int> GetAppliedStateSpawnRate(FarmPlotSpawnStateRate spawnRate, List<int> indicesLeft)
    {
        List<int> chosenIndices = new List<int>();
        List<int> indexOptions = new List<int>(indicesLeft); //copy of indicesLeft
        if(spawnRate.finalPlotAmount != -1)
        {
            for(int i = 0; i < spawnRate.finalPlotAmount; ++i)
            {
                if (indexOptions.Count == 0) break;
                int index = UnityEngine.Random.Range(0, indexOptions.Count);
                chosenIndices.Add(indexOptions[index]);
                indexOptions.RemoveAt(index);
            }
            return chosenIndices;
        }
        //first at the min amount of spawns
        for (int i = 0; i < spawnRate.minSpawns; ++i)
        {
            if (indexOptions.Count == 0) break;
            int index = UnityEngine.Random.Range(0, indexOptions.Count);
            chosenIndices.Add(indexOptions[index]);
            indexOptions.RemoveAt(index);
        }
        //then do it randomly until all plots have been gone through or max is reached
        for(int i = 0; i < indexOptions.Count; ++i)
        {
            if (spawnRate.maxSpawns == chosenIndices.Count) break;
            int rand = UnityEngine.Random.Range(0, 101);
            if(rand <= spawnRate.spawnRate)
            {
                chosenIndices.Add(indexOptions[i]);
                indexOptions.RemoveAt(i);
            }
        }

        chosenIndices.Sort();
        return chosenIndices;
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

    public void OnDrag(Vector3 position, IControllable dragged, ControllerHitInfo hitInfo)
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
        if(state == FarmPlot.State.Growing)
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
        random.x = UnityEngine.Random.Range(1, 2);
        random.y = 1;
        random.z = UnityEngine.Random.Range(1, 2);
        if (UnityEngine.Random.Range(-1, 1) < 0) random.x *= -1; 
        if (UnityEngine.Random.Range(-1, 1) < 0) random.z *= -1; 
        return random * 2.5f;
    }

    public void OnPlotHarvest(FarmPlot plot)
    {
    }
}
