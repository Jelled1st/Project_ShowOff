﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class FarmTutorial : MonoBehaviour, IFarmPlotObserver, ISubject
{
    [SerializeField]
    private TextMeshProUGUI _shovelQuest;

    [SerializeField]
    private TextMeshProUGUI _plantQuest;

    [SerializeField]
    private TextMeshProUGUI _waterQuest;

    [Header("First bug kill")]
    [SerializeField]
    private GameObject _bugSwipeAnimation;

    [Header("First spray")]
    [SerializeField]
    private GameObject _sprayHand;

    [SerializeField]
    private GameObject _sprayTool;

    [Header("FirstHarvest")]
    [SerializeField]
    private GameObject _harvestHand;

    [SerializeField]
    private GameObject _truck;

    [Header("Events")]
    [SerializeField]
    private UnityEvent _onStart;

    [SerializeField]
    private UnityEvent _completeShovelQuestEvent;

    [SerializeField]
    private UnityEvent _startPlantQuestEvent;

    [SerializeField]
    private UnityEvent _completePlantQuestEvent;

    [SerializeField]
    private UnityEvent _startWaterQuestEvent;

    [SerializeField]
    private UnityEvent _completeWaterQuestEvent;

    [SerializeField]
    private UnityEvent _firstBugSpawnEvent;

    [SerializeField]
    private UnityEvent _completeBugKillEvent;

    [SerializeField]
    private UnityEvent _failBugKillEvent;

    [SerializeField]
    private FarmPlot _tutorialPlot;

    private bool _shovelComplete = false;
    private bool _plantQuestStarted = false;
    private bool _plantComplete = false;
    private bool _waterQuestStarted = false;
    private bool _waterComplete = false;
    private bool _killBugsCompelte = false;
    private bool _firstBug = true;

    private bool _firstHarvest = true;
    private bool _firstHarvestCompleted = false;
    private Vector2 _harvestTweenStart;

    private bool _firstSpray = true;
    private bool _firstSprayCompleted = false;
    private Vector2 _sprayTweenEnd;

    private List<IObserver> _observers = new List<IObserver>();
    private List<FarmPlot> _farmPlots = new List<FarmPlot>();


    private bool _firstUpdateCalled = false;

    public void Init()
    {
        SubscribeAndDisableFarmPlots();
        Swarm.RegisterStatic(this);
        ResetTutorial();
    }

    private void SubscribeAndDisableFarmPlots()
    {
        var farmPlotsGOs = GameObject.FindGameObjectsWithTag("FarmPlot");
        for (var i = 0; i < farmPlotsGOs.Length; ++i)
        {
            var farmPlot = farmPlotsGOs[i].GetComponent<FarmPlot>();
            if (farmPlot != _tutorialPlot)
            {
                farmPlot.SetInteractable(false);
            }
            else
            {
                farmPlot.SetInteractable(true);
                farmPlot.SetStartState(FarmPlot.State.Rough);
            }

            Subscribe(farmPlot);
            _farmPlots.Add(farmPlot);
        }
    }

    private void ResetTutorial()
    {
        _shovelComplete = false;
        _plantQuestStarted = false;
        _plantComplete = false;
        _waterQuestStarted = false;
        _waterComplete = false;
        _killBugsCompelte = false;
        _firstBug = true;

        _firstHarvest = true;
        _firstHarvestCompleted = false;
        _harvestTweenStart = new Vector2();

        _firstSpray = true;
        _firstSprayCompleted = false;
        _sprayTweenEnd = new Vector2();
    }

    private void EnableFarmPlots()
    {
        for (var i = 0; i < _farmPlots.Count; ++i)
        {
            _farmPlots[i].SetInteractable(true);
        }
    }

    private void Update()
    {
        if (!_firstUpdateCalled)
        {
            _onStart.Invoke();
            ResetTutorial();
            _firstUpdateCalled = true;
        }

        if (!_firstHarvest && !_firstHarvestCompleted)
        {
            if (!DOTween.IsTweening(_harvestHand.transform))
            {
                _harvestHand.transform.position = _harvestTweenStart;
                Vector2 truckPos = Camera.main.WorldToScreenPoint(_truck.transform.position);
                _harvestHand.transform.DOMove(truckPos, 1.5f);
            }
        }

        if (!_firstSpray && !_firstSprayCompleted)
        {
            if (!DOTween.IsTweening(_sprayHand.transform))
            {
                _sprayHand.transform.position = _sprayTool.transform.position;
                _sprayHand.transform.DOMove(_sprayTweenEnd, 1.5f);
            }
        }
    }

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
        if (observerEvent is SwarmSpawnEvent)
        {
            Subscribe((observerEvent as SwarmSpawnEvent).swarm);
        }

        if (observerEvent is SwarmBugSpawnEvent)
        {
            if (_firstBug)
            {
                _firstBug = false;
                _firstBugSpawnEvent.Invoke();
                if (_bugSwipeAnimation != null) _bugSwipeAnimation.SetActive(true);
                var bugPoint =
                    Camera.main.WorldToScreenPoint((observerEvent as SwarmBugSpawnEvent).bug.transform.position);
                if (_bugSwipeAnimation != null)
                    _bugSwipeAnimation.transform.position = new Vector3(bugPoint.x, bugPoint.y, 0);
            }
        }

        if (observerEvent is SwarmBugKillEvent)
        {
            _killBugsCompelte = true;
            _completeBugKillEvent.Invoke();
            CheckCompletion();
            if (_bugSwipeAnimation != null) _bugSwipeAnimation.SetActive(false);
        }
    }

    public void OnPlotHarvest(FarmPlot plot)
    {
    }

    public void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot)
    {
        if (switchState == FarmPlot.State.Dug && !_shovelComplete)
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
            Debug.Log("Start plant quest");
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
                Vector2 truckPos = Camera.main.WorldToScreenPoint(_truck.transform.position);
                _harvestHand.transform.position = _harvestTweenStart;
                _harvestHand.transform.DOMove(truckPos, 1.5f);
            }
        }
        else if (state == FarmPlot.State.Harvested)
        {
            if (!_firstHarvestCompleted)
            {
                _firstHarvestCompleted = true;
                DOTween.Kill(_harvestHand.transform);
                _harvestHand.SetActive(false);
            }
        }
        else if (state == FarmPlot.State.Decay)
        {
            if (_firstSpray)
            {
                _firstSpray = false;
                _sprayHand.SetActive(true);
                _sprayTweenEnd = Camera.main.WorldToScreenPoint(plot.transform.position);
                _sprayHand.transform.position = _sprayTool.transform.position;
                _sprayHand.transform.DOMove(_sprayTweenEnd, 1.5f);
            }
        }
        else if (state == FarmPlot.State.Healing)
        {
            if (!_firstSprayCompleted)
            {
                _firstSprayCompleted = true;
                DOTween.Kill(_sprayHand.transform);
                _sprayHand.SetActive(false);
            }
        }
        else if (state == FarmPlot.State.Withered)
        {
            if (!_firstSprayCompleted)
            {
                _firstSprayCompleted = true;
                DOTween.Kill(_sprayHand.transform);
                _sprayHand.SetActive(false);
                _failBugKillEvent.Invoke();
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
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }

    #endregion
}