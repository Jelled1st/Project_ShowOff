using System.Collections.Generic;
using UnityEngine;

public class FarmPlotAnimatedToolsHandler : MonoBehaviour, IFarmPlotObserver
{
    [SerializeField] private List<GameObject> _farmTools;
    [SerializeField] private List<FarmPlot.State> _playStates;
    [SerializeField] private List<FarmPlot.State> _playAfterStates;
    [SerializeField] private List<Vector3> _spawnOffset;

    private Dictionary<FarmPlot, GameObject> _farmPlotAnimatedTools = new Dictionary<FarmPlot, GameObject>();
        
    // Start is called before the first frame update
    void Start()
    {
        if (_farmTools.Count != _playStates.Count || _farmTools.Count != _spawnOffset.Count || _farmTools.Count != _playAfterStates.Count) Debug.Log("WARNING: farm tools not equal to animation states or spawn offsets!");

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

    private void SpawnAnimatedTool(FarmPlot plot, FarmPlot.State state, FarmPlot.State currentState)
    {
        for(int i = 0; i < _playStates.Count; ++i)
        {
            if(_playStates[i] == state && (_playAfterStates[i] == currentState || _playAfterStates[i] == FarmPlot.State.Undifined))
            {
                GameObject farmTool = Instantiate(_farmTools[i]);
                _farmPlotAnimatedTools.Add(plot, farmTool);
                farmTool.GetComponent<Animator>().SetBool("isPlaying", true);
                farmTool.transform.position = plot.transform.position + _spawnOffset[i];
                break;
            }
        }
    }

    private void DestroyAnimatedTool(FarmPlot plot)
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
        SpawnAnimatedTool(plot, switchState, currentState);
    }

    public void OnPlotStateSwitch(FarmPlot.State state, FarmPlot.State previousState, FarmPlot plot)
    {
        DestroyAnimatedTool(plot);
    }

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
    }
    #endregion
}
