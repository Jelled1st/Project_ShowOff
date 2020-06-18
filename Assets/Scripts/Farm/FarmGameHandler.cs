using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class FarmGameHandler : MonoBehaviour, ISubject, IControlsObserver, IFarmPlotObserver, ISwarmObserver
{
    [SerializeField] GameObject _swarmPrefab;
    private TouchController _touchController;
    [Header("SpawnStates and rates")]
    [SerializeField] List<FarmPlotSpawnStateRate> _stateSpawnRates;
    [SerializeField] private string _nextScene = "";

    [Header("Scores")]
    [SerializeField] private float _harvestPoints = 250;
    [SerializeField] private float _fullyHealthyPoints = 500;
    [SerializeField] private float _killBugPoinst = 100;
    [SerializeField] private float _decayPenaltyPoints = 0;
    [SerializeField] private float _witherPenaltyPoints = 25;

    private List<IGameHandlerObserver> _gameHandlerObservers;
    private List<FarmPlot> _farmPlots = new List<FarmPlot>();
    private bool _paused = false;
    private bool _gameFinished = false;
    private bool _debugLog = false;
    bool _trulyFinished = false;

    private bool _plantsStartAtGrown = false;

    [Header("Tutorial")]
    [SerializeField] private bool _doTutorial = true;
    private bool _pausedForTutorial = false;
    private bool _repeatPause = false;
    private bool _repeatPauseForTutorial = false;

    void Awake()
    {
        this.gameObject.tag = "GameHandler";
        if (_nextScene == "")
        {
            _nextScene = SceneManager.GetActiveScene().name;
            Debug.LogWarning("Next scene not set, reloading this scene assumed");
        }
        Swarm.RegisterStatic(this);
    }

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
        for (int i = 0; i < farmPlotsGOs.Length; ++i)
        {
            _farmPlots.Add(farmPlotsGOs[i].GetComponent<FarmPlot>());
        }

        if (_plantsStartAtGrown)
        {
            for (int i = 0; i < _farmPlots.Count; ++i)
            {
                _farmPlots[i].SetStartState(FarmPlot.State.Grown);
            }
        }
        else SetFarmPlotStates(_farmPlots);

        for (int i = 0; i < _farmPlots.Count; ++i)
        {
            Subscribe(_farmPlots[i]);
        }
    }

    private void SetFarmPlotStates(List<FarmPlot> farmPlots)
    {
        if(_stateSpawnRates == null || _stateSpawnRates.Count == 0)
        {
            for(int i = 0; i < farmPlots.Count; ++i)
            {
                farmPlots[i].SetStartState(FarmPlot.State.Rough);
            }
            return;
        }

        List<int> farmPlotsToBeSet = new List<int>();
        for (int i = 0; i < farmPlots.Count; ++i)
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
        if (_repeatPause)
        {
            Pause(_repeatPauseForTutorial);
        }
        if (_pausedForTutorial && Input.GetMouseButtonDown(0))
        {
            UnPause();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FinishGame();
        }
    }

    public void LastPotatoCollected()
    {
        FinishGame();
    }

    public void RepeatPause(bool tutorialPause = false)
    {
        _repeatPause = true;
        _repeatPauseForTutorial = tutorialPause;
        Pause(tutorialPause);
    }

    public void RepeatUnPause()
    {
        _repeatPause = false;
        UnPause();
    }

    public void Pause(bool tutorialPause)
    {
        _pausedForTutorial = tutorialPause;
        if (!_paused)
        {
            _paused = true;
            for (int i = 0; i < _gameHandlerObservers.Count; ++i)
            {
                _gameHandlerObservers[i].OnPause();
            }
        }
    }

    public void UnPause()
    {
        if (_paused)
        {
            _pausedForTutorial = false;
            _paused = false;
            for (int i = 0; i < _gameHandlerObservers.Count; ++i)
            {
                _gameHandlerObservers[i].OnContinue();
            }
        }
    }

    public void FinishGame()
    {
        if (!_gameFinished)
        {
            _gameFinished = true;
            for (int i = 0; i < _gameHandlerObservers.Count; ++i)
            {
                _gameHandlerObservers[i].OnFinish();
            }
        }
        StartCoroutine(LoadScene());
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("crossed");
        Truck truck;
        if(other.TryGetComponent<Truck>(out truck) && _gameFinished)
        {
            //Game truly finished
            //SceneManager.LoadScene(_nextScene);
            _trulyFinished = true;
        }
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_nextScene);

        asyncOperation.allowSceneActivation = false;
        
        while (!asyncOperation.isDone)
        {
            print("Loading Progress: " + (asyncOperation.progress * 100) + "%");
            if(asyncOperation.progress >= 0.9f)
            {
                if(_trulyFinished == true)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    #region ControlsObserver
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
    #endregion

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    public void OnNotify(AObserverEvent observerEvent)
    {
        if(observerEvent is SwarmSpawnEvent)
        {
            Subscribe((observerEvent as SwarmSpawnEvent).swarm);
        }
    }

    #region IFarmPlotObserver
    public void OnPlotStartStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        if(state == FarmPlot.State.Decay)
        {
            Scores.SubScore(_decayPenaltyPoints);
        }
        else if (state == FarmPlot.State.Withered)
        {
            Scores.SubScore(_witherPenaltyPoints);
        }
        else if(state == FarmPlot.State.Growing)
        {
            GameObject swarmGO = Instantiate(_swarmPrefab);
            Swarm swarm = swarmGO.GetComponent<Swarm>();
            swarm.Init(plot);
            swarmGO.transform.position = plot.gameObject.transform.position;
            Subscribe(swarm);
        }
        else if(state == FarmPlot.State.Grown)
        {
            if (!plot.HasBeenPoisened()) Scores.AddScore(_fullyHealthyPoints);
        }
    }


    public void OnPlotHarvest(FarmPlot plot)
    {
        Scores.AddScore(_harvestPoints);
    }
    #endregion

    public void Register(IObserver observer)
    {
        if (_gameHandlerObservers == null) _gameHandlerObservers = new List<IGameHandlerObserver>();
        if (observer is IGameHandlerObserver)
        {
            _gameHandlerObservers.Add(observer as IGameHandlerObserver);
        }
    }

    public void UnRegister(IObserver observer)
    {
        if (observer is IGameHandlerObserver)
        {
            _gameHandlerObservers.Remove(observer as IGameHandlerObserver);
        }
    }

    public void Notify(AObserverEvent observerEvent)
    {
    }

    #region ISwarmObserver

    public void OnBugKill(SwarmUnit unit)
    {
        Scores.AddScore(_killBugPoinst);
    }

    public void OnSwarmDestroy()
    {
    }

    public void OnFlee()
    {
    }

    public void OnBugSpawn(SwarmUnit unit)
    {
    }

    public void OnBugspawnFail()
    {
    }
    #endregion
}
