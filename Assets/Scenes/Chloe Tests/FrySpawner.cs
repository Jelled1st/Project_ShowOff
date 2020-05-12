using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrySpawner : MonoBehaviour
{
    public GameObject[] fries;
    public float rotationSpeed;
    public float force;
    public int amount;
    public float timeToLive;
    private GameObject clone;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                gameObject.transform.position = hit.point;
            }
                for (int i = 0; i < amount; i++)
                {
                Destroy(Instantiate(fries[Random.Range(0, fries.Length)], transform.position, transform.rotation), timeToLive);
                        
                    transform.Rotate(new Vector3(0, rotationSpeed, 0), Space.World);
                for (int a = 0; a < fries.Length; a++)
                {
                    fries[a].GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(force - 200, force + 200));
                }
            }

        }
    }
}