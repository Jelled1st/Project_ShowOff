﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class FarmGameHandler : MonoBehaviour, ISubject, IControlsObserver, IFarmPlotObserver, ISwarmObserver
{
    [SerializeField]
    private GameObject _swarmPrefab;

    private TouchController _touchController;

    [Header("SpawnStates and rates")]
    [SerializeField]
    private List<FarmPlotSpawnStateRate> _stateSpawnRates;

    [SerializeField]
    private string _nextScene = "";

    public GameObject blackOutSquare;
    public GameObject blackoutCanvas;

    [Header("Scores")]
    [SerializeField]
    private float _harvestPoints = 250;

    [SerializeField]
    private float _fullyHealthyPoints = 500;

    [SerializeField]
    private float _killBugPoinst = 100;

    [SerializeField]
    private float _decayPenaltyPoints = 0;

    [SerializeField]
    private float _witherPenaltyPoints = 25;

    private List<IGameHandlerObserver> _gameHandlerObservers;
    private List<FarmPlot> _farmPlots = new List<FarmPlot>();
    private bool _paused = false;
    private bool _gameFinished = false;
    private bool _debugLog = false;
    private bool _trulyFinished = false;

    private bool _plantsStartAtGrown = false;

    [Header("Tutorial")]
    [SerializeField]
    private FarmTutorial _turorial;

    [SerializeField]
    private bool _doTutorial = true;

    private bool _pausedForTutorial = false;
    private bool _repeatPause = false;
    private bool _repeatPauseForTutorial = false;

    private void Awake()
    {
        gameObject.tag = "GameHandler";
        if (_nextScene == "")
        {
            _nextScene = SceneManager.GetActiveScene().name;
            Debug.LogWarning("Next scene not set, reloading this scene assumed");
        }

        Swarm.RegisterStatic(this);
        if (blackoutCanvas.activeInHierarchy == false)
        {
            blackoutCanvas.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn());

        var controller = GameObject.FindGameObjectWithTag("Controller");
        if (controller == null)
        {
            Debug.LogError("No controller found!");
        }
        else
        {
            _touchController = controller.GetComponent<TouchController>();
            Subscribe(_touchController);
        }

        var farmPlotsGOs = GameObject.FindGameObjectsWithTag("FarmPlot");
        //Set states before substribing
        for (var i = 0; i < farmPlotsGOs.Length; ++i)
        {
            _farmPlots.Add(farmPlotsGOs[i].GetComponent<FarmPlot>());
        }

        if (_plantsStartAtGrown)
        {
            for (var i = 0; i < _farmPlots.Count; ++i)
            {
                _farmPlots[i].SetStartState(FarmPlot.State.Grown);
            }
        }
        else
        {
            SetFarmPlotStates(_farmPlots);
        }

        _turorial.Init();
        for (var i = 0; i < _farmPlots.Count; ++i)
        {
            Subscribe(_farmPlots[i]);
        }
    }

    private void SetFarmPlotStates(List<FarmPlot> farmPlots)
    {
        if (_stateSpawnRates == null || _stateSpawnRates.Count == 0)
        {
            for (var i = 0; i < farmPlots.Count; ++i)
            {
                farmPlots[i].SetStartState(FarmPlot.State.Rough);
            }

            return;
        }

        var farmPlotsToBeSet = new List<int>();
        for (var i = 0; i < farmPlots.Count; ++i)
        {
            farmPlotsToBeSet.Add(i);
        }

        _stateSpawnRates.Sort();
        for (var i = 0; i < _stateSpawnRates.Count; ++i)
        {
            if (farmPlotsToBeSet.Count == 0) break;
            if (_debugLog) Debug.Log("Applying spawn rate " + _stateSpawnRates[i].state);
            var chosenIndeces = GetAppliedStateSpawnRate(_stateSpawnRates[i], farmPlotsToBeSet);
            for (var j = 0; j < chosenIndeces.Count; ++j)
            {
                if (_debugLog) Debug.Log("Chosen index[" + j + "] = " + chosenIndeces[j]);
                farmPlots[chosenIndeces[j]].SetStartState(_stateSpawnRates[i].state);
            }

            for (var j = chosenIndeces.Count - 1; j >= 0; --j)
            {
                farmPlotsToBeSet.Remove(chosenIndeces[j]);
            }
        }
    }

    //returns a list of chosen indecis by rng
    private List<int> GetAppliedStateSpawnRate(FarmPlotSpawnStateRate spawnRate, List<int> indicesLeft)
    {
        var chosenIndices = new List<int>();
        var indexOptions = new List<int>(indicesLeft); //copy of indicesLeft
        if (spawnRate.finalPlotAmount != -1)
        {
            for (var i = 0; i < spawnRate.finalPlotAmount; ++i)
            {
                if (indexOptions.Count == 0) break;
                var index = Random.Range(0, indexOptions.Count);
                chosenIndices.Add(indexOptions[index]);
                indexOptions.RemoveAt(index);
            }

            return chosenIndices;
        }

        //first at the min amount of spawns
        for (var i = 0; i < spawnRate.minSpawns; ++i)
        {
            if (indexOptions.Count == 0) break;
            var index = Random.Range(0, indexOptions.Count);
            chosenIndices.Add(indexOptions[index]);
            indexOptions.RemoveAt(index);
        }

        //then do it randomly until all plots have been gone through or max is reached
        for (var i = 0; i < indexOptions.Count; ++i)
        {
            if (spawnRate.maxSpawns == chosenIndices.Count) break;
            var rand = Random.Range(0, 101);
            if (rand <= spawnRate.spawnRate)
            {
                chosenIndices.Add(indexOptions[i]);
                indexOptions.RemoveAt(i);
            }
        }

        chosenIndices.Sort();
        return chosenIndices;
    }

    private void Update()
    {
        if (_repeatPause)
        {
            Pause(_repeatPauseForTutorial);
        }

        if (_pausedForTutorial && Input.GetMouseButtonDown(0))
        {
            UnPause();
        }

        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            FinishGame();
        }*/
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
            for (var i = 0; i < _gameHandlerObservers.Count; ++i)
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
            for (var i = 0; i < _gameHandlerObservers.Count; ++i)
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
            for (var i = 0; i < _gameHandlerObservers.Count; ++i)
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
        if (other.TryGetComponent<Truck>(out truck) && _gameFinished)
        {
            //Game truly finished
            _trulyFinished = true;
        }
    }

    private IEnumerator LoadScene(int fadeSpeed = 5)
    {
        yield return null;
        //declare color for fade out
        var objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        var asyncOperation = SceneManager.LoadSceneAsync(_nextScene);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            print("Loading Progress: " + asyncOperation.progress * 100 + "%");
            if (asyncOperation.progress >= 0.9f)
            {
                if (_trulyFinished == true)
                {
                    //fade out first
                    while (blackOutSquare.GetComponent<Image>().color.a < 1)
                    {
                        fadeAmount = objectColor.a + fadeSpeed * Time.deltaTime;

                        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                        blackOutSquare.GetComponent<Image>().color = objectColor;
                        yield return null;
                    }

                    //then activate new scene
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public IEnumerator FadeIn(bool fadeToWhite = true, int fadeSpeed = 5)
    {
        var objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToWhite)
        {
            while (blackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - fadeSpeed * Time.deltaTime;

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
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
        if (observerEvent is SwarmSpawnEvent)
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
        if (state == FarmPlot.State.Decay)
        {
            Scores.SubScore(_decayPenaltyPoints);
        }
        else if (state == FarmPlot.State.Withered)
        {
            Scores.SubScore(_witherPenaltyPoints);
        }
        else if (state == FarmPlot.State.Growing)
        {
            var swarmGO = Instantiate(_swarmPrefab);
            var swarm = swarmGO.GetComponent<Swarm>();
            swarm.Init(plot);
            swarmGO.transform.position = plot.gameObject.transform.position;
            Subscribe(swarm);
        }
        else if (state == FarmPlot.State.Grown)
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