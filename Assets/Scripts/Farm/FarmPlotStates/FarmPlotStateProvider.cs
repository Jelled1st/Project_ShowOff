using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlotStateProvider : MonoBehaviour
{
    [SerializeField] List<FarmPlot.State> _farmplotStates;
    [SerializeField] List<FarmPlotState> _stateObjects;
    [SerializeField] FarmPlotState _nullState;
    private Dictionary<FarmPlot.State, FarmPlotState> _states;

    private void Start()
    {
        _states = new Dictionary<FarmPlot.State, FarmPlotState>();
        if (_farmplotStates.Count == _stateObjects.Count)
        {
            for (int i = 0; i < _farmplotStates.Count; ++i)
            {
                _states.Add(_farmplotStates[i], _stateObjects[i]);
            }
        }
        else Debug.Log("Farmplot states count does not equal provided state objects count");
    }

    public FarmPlotState RequestStateObjectForState(FarmPlot.State state)
    {
        if (_states.ContainsKey(state)) return _states[state];
        else return _nullState;
    }

    public FarmPlotState GetNullState()
    {
        return _nullState;
    }
}
