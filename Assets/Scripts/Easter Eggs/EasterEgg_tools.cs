using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg_tools : MonoBehaviour
{
    [SerializeField] private Rigidbody _tool1;
    [SerializeField] private Rigidbody _tool2;
    [SerializeField] private Rigidbody _tool3;
    void Start()
    {
        _tool1.constraints = RigidbodyConstraints.FreezeAll;
        _tool2.constraints = RigidbodyConstraints.FreezeAll;
        _tool3.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnMouseDown()
    {
        //disable frozen
        _tool1.constraints = RigidbodyConstraints.None;
        _tool2.constraints = RigidbodyConstraints.None;
        _tool3.constraints = RigidbodyConstraints.None;

        //disable gravity on tools
        _tool1.useGravity = false;
        _tool2.useGravity = false;
        _tool3.useGravity = false;

        //add force upwards
        _tool1.AddForce(0, Random.Range(50f, 70f), 0);
        _tool2.AddForce(0, Random.Range(50f, 70f), 0);
        _tool3.AddForce(0, Random.Range(50f, 70f), 0);

        //add angular velocity
        _tool1.AddRelativeTorque(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        _tool1.AddRelativeTorque(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        _tool1.AddRelativeTorque(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
}
