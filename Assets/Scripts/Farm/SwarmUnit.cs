using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmUnit : MonoBehaviour
{
    private Swarm _swarm;

    public void Init(Swarm swarm)
    {
        _swarm = swarm;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        FarmPlot plot;
        if (other.gameObject.TryGetComponent<FarmPlot>(out plot))
        {
            _swarm.UnitEnterPlot(this, plot);
        }
    }
}
