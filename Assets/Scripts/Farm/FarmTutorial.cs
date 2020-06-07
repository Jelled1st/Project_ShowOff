using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class FarmTutorial : MonoBehaviour, IFarmPlotObserver, ISubject
{
    [SerializeField] TextMeshProUGUI _shovelQuest;
    [SerializeField] TextMeshProUGUI _plantQuest;
    [SerializeField] TextMeshProUGUI _waterQuest;
    [Header("First bug kill")]
    [SerializeField] GameObject _bugSwipeAnimation;
    [Header("FirstHarvest")]
    [SerializeField] GameObject _harvestHand;
    [SerializeField] Vector2 _truckPosition = new Vector2(270, 220);
    [Header("Events")]
    [SerializeField] UnityEvent _completeShovelQuestEvent;
    [SerializeField] UnityEvent _startPlantQuestEvent;
    [SerializeField] UnityEvent _completePlantQuestEvent;
    [SerializeField] UnityEvent _startWaterQuestEvent;
    [SerializeField] UnityEvent _completeWaterQuestEvent;
    [SerializeField] UnityEvent _firstBugSpawnEvent;
    [SerializeField] UnityEvent _completeBugKillEvent;
    [SerializeField] FarmPlot _tutorialPlot;

    private bool _shovelComplete = false;
    private bool _plantQuestStarted = false;
    private bool _plantComplete = false;
    private bool _waterQuestStarted = false;
    private bool _waterComplete = false;
    private bool _killBugsCompelte = false;
    private bool _firstBug = true;

    private bool _firstHarvest = true;
    private bool _completedFirstHarvest = false;
    private Vector2 _harvestTweenStart;

    private List<IObserver> _observers = new List<IObserver>();
    private List<FarmPlot> _farmPlots = new List<FarmPlot>();

    // Start is called before the first frame update
    void Start()
    {
        SubscribeAndDisableFarmPlots();
        Swarm.RegisterStatic(this);
    }

    private void SubscribeAndDisableFarmPlots()
    {
        GameObject[] farmPlotsGOs = GameObject.FindGameObjectsWithTag("FarmPlot");
        for (int i = 0; i < farmPlotsGOs.Length; ++i)
        {
            FarmPlot farmPlot = farmPlotsGOs[i].GetComponent<FarmPlot>();
            Subscribe(farmPlot);
            if (farmPlot != _tutorialPlot) farmPlot.SetInteractable(false);
            else
            {
                farmPlot.SetInteractable(true);
                farmPlot.SetStartState(FarmPlot.State.Rough);
            }
            _farmPlots.Add(farmPlot);
        }
    }

    private void EnableFarmPlots()
    {
        for(int i = 0; i < _farmPlots.Count; ++i)
        {
            _farmPlots[i].SetInteractable(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_firstHarvest && !_completedFirstHarvest)
        {
            if(!DOTween.IsTweening(_harvestHand.transform))
            {
                _harvestHand.transform.position = _harvestTweenStart;
                _harvestHand.transform.DOMove(_truckPosition, 1.5f);
            }
        }
    }

    //private Vector2 CalcCanvasBugPosition(SwarmUnit bug, )
    //{

    //}

    private void CheckCompletion()
    {
        if (_shovelComplete && _plantComplete && _waterComplete && _killBugsCompelte)
        {
            Notify(new FarmTutorialCompleteEvent(this));
        }
    }

    #region IFarmPlotObserver
    public void OnNotify(AObserverEvent observerEvent)
    {
        if(observerEvent is SwarmSpawnEvent)
        {
            Subscribe((observerEvent as SwarmSpawnEvent).swarm);
        }
        if(observerEvent is SwarmBugSpawnEvent)
        {
            if(_firstBug)
            {
                _firstBug = false;
                _firstBugSpawnEvent.Invoke();
                _bugSwipeAnimation.SetActive(true);
                Vector3 bugPoint = Camera.main.WorldToScreenPoint((observerEvent as SwarmBugSpawnEvent).bug.transform.position);
                _bugSwipeAnimation.transform.position = new Vector3(bugPoint.x, bugPoint.y, 0);
            }
        }
        if(observerEvent is SwarmBugKillEvent)
        {
            _killBugsCompelte = true;
            _completeBugKillEvent.Invoke();
            CheckCompletion();
            _bugSwipeAnimation.SetActive(false);
        }
    }

    public void OnPlotHarvest(FarmPlot plot)
    {
    }

    public void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot)
    {
        if(switchState == FarmPlot.State.Dug && !_shovelComplete)
        {
            _shovelComplete = true;
            _shovelQuest.fontStyle = FontStyles.Strikethrough;
            _completeShovelQuestEvent.Invoke();
            CheckCompletion();
        }
        else if (switchState == FarmPlot.State.Planted && !_plantComplete)
        {
            _plantComplete = true;
            _plantQuest.fontStyle = FontStyles.Strikethrough;
            _completePlantQuestEvent.Invoke();
            CheckCompletion();
        }
        else if (switchState == FarmPlot.State.Growing && !_waterComplete)
        {
            _waterComplete = true;
            _waterQuest.fontStyle = FontStyles.Strikethrough;
            _completeWaterQuestEvent.Invoke();
            CheckCompletion();
        }
        else if (switchState == FarmPlot.State.Grown)
        {
        }



        if (_shovelComplete && _plantComplete && _waterComplete)
        {
            Notify(new FarmTutorialPlotCompleteEvent(this));
            EnableFarmPlots();
        }
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        if (state == FarmPlot.State.Dug && !_plantQuestStarted)
        {
            _plantQuestStarted = true;
            _startPlantQuestEvent.Invoke();
        }
        else if (state == FarmPlot.State.Planted && !_waterQuestStarted)
        {
            _waterQuestStarted = true;
            _startWaterQuestEvent.Invoke();
        }
        else if (state == FarmPlot.State.Growing)
        {
        }
        else if (state == FarmPlot.State.Grown)
        {
            if (_firstHarvest)
            {
                _firstHarvest = false;
                _harvestHand.SetActive(true);
                _harvestTweenStart = Camera.main.WorldToScreenPoint(plot.transform.position);
                _harvestHand.transform.position = _harvestTweenStart;
                _harvestHand.transform.DOMove(new Vector3(_truckPosition.x, _truckPosition.y, 0), 1.5f);
            }
        }
        else if (state == FarmPlot.State.Harvested)
        {
            if (!_completedFirstHarvest)
            {
                _completedFirstHarvest = true;
                DOTween.Kill(_harvestHand.transform);
                _harvestHand.SetActive(false);
            }
        }
    }

    public void Subscribe(ISubject subject)
    {
        subject.Register(this);
    }

    public void UnSubscribe(ISubject subject)
    {
        subject.UnRegister(this);
    }

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
    #endregion
}
