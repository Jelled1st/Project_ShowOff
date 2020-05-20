using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class CuttableFood : MonoBehaviour
{
    [SerializeField] private GameObject _currentState;
    [SerializeField] private List<GameObject> _cutStates;
    int _currentStateIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(_cutStates != null && _cutStates[0] !=_currentState)
        {
            _cutStates.Insert(0, _currentState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Cut()
    {
        if (_currentStateIndex < _cutStates.Count - 1)
        {
            Destroy(_currentState);
            _currentState = Instantiate(_cutStates[++_currentStateIndex], this.transform);
            _currentState.transform.localPosition = new Vector3(0, 0, 0);
            return true;
        }
        else return false;
    }
}
