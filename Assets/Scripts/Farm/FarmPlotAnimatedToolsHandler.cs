using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlotAnimatedToolsHandler : MonoBehaviour, IFarmPlotObserver
{
    [SerializeField] private List<GameObject> _farmTools;
    [SerializeField] private List<FarmPlot.State> _farmToolState;

    private Dictionary<FarmPlot, GameObject> _farmPlotAnimatedTools = new Dictionary<FarmPlot, GameObject>();
        
    // Start is called before the first frame update
    void Start()
    {
        if (_farmTools.Count != _farmToolState.Count) Debug.Log("WARNING: farm tools not equal to play states");

        //subscribe to all farm plots
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

    private void SpawnAnimatedTool(FarmPlot plot, FarmPlot.State state)
    {
        for(int i = 0; i < _farmToolState.Count; ++i)
        {
            if(_farmToolState[i] == state)
            {
                GameObject farmTool = Instantiate(_farmTools[i]);
                _farmPlotAnimatedTools.Add(plot, farmTool);
                //farmTool.GetComponent<>().Play();
                farmTool.transform.position = plot.transform.position;
                break;
            }
        }
    }

    private void DestroyAnimatedTool(FarmPlot plot, FarmPlot.State state)
    {
        if (_farmPlotAnimatedTools.ContainsKey(plot))
        {
            GameObject tool = _farmPlotAnimatedTools[plot];
            _farmPlotAnimatedTools.Remove(plot);
            Destroy(tool);
        }
    }

    #region IFarmPlotObserver
    public void OnPlotHarvest(FarmPlot plot)
    {
    }

    public void OnPlotStartStateSwitch(FarmPlot.State switchState, FarmPlot.State currentState, FarmPlot plot)
    {
        SpawnAnimatedTool(plot, switchState);
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        DestroyAnimatedTool(plot, state);
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
