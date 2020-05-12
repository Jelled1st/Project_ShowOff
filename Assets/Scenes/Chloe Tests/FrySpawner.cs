using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrySpawner : MonoBehaviour
{
    public GameObject[] fries;
    public float rotationSpeed;
    public float force;

    // Update is called once per frame
    void Update()
    {
        Instantiate(fries[Random.Range(0, fries.Length)], transform.position, transform.rotation)
            .GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(force - 200, force + 200));

        transform.Rotate(new Vector3(0, rotationSpeed, 0), Space.World);
    }
}