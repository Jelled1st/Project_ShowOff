using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class FarmTutorial : MonoBehaviour, IFarmPlotObserver
{
    [SerializeField] TextMeshProUGUI _shovelQuest;
    [SerializeField] TextMeshProUGUI _plantQuest;
    [SerializeField] TextMeshProUGUI _waterQuest;
    [SerializeField] UnityEvent _firstBugSpawnEvents;

    private bool _shovelComplete = false;
    private bool _plantComplete = false;
    private bool _waterComplete = false;
    private bool _firstBug = true;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToFarmPlots();
        Swarm.RegisterStatic(this);
    }

    private void SubscribeToFarmPlots()
    {
        GameObject[] farmPlotsGOs = GameObject.FindGameObjectsWithTag("FarmPlot");
        for (int i = 0; i < farmPlotsGOs.Length; ++i)
        {
            FarmPlot farmPlot = farmPlotsGOs[i].GetComponent<FarmPlot>();
            Subscribe(farmPlot);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
                _firstBugSpawnEvents.Invoke();
            }
        }
    }

    public void OnPlotHarvest(FarmPlot plot)
    {
    }

    public void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot)
    {
        if(switchState == FarmPlot.State.Dug)
        {
            _shovelComplete = true;
            _shovelQuest.fontStyle = FontStyles.Strikethrough;
        }
        else if (switchState == FarmPlot.State.Planted)
        {
            _plantComplete = true;
            _plantQuest.fontStyle = FontStyles.Strikethrough;
        }
        else if (switchState == FarmPlot.State.Growing)
        {
            _waterComplete = true;
            _waterQuest.fontStyle = FontStyles.Strikethrough;
        }
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
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
    #endregion
}
