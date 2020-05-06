using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatConveyorBelt : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] Material conveyorMaterial;

    Rigidbody rBody;
    // Start is called before the first frame update
    void Start()
    {
        if(!TryGetComponent<Rigidbody>(out rBody))
        {
            rBody = this.gameObject.AddComponent<Rigidbody>();
        }
        rBody.useGravity = true;
        rBody.isKinematic = true;
        conveyorMaterial.SetFloat("_scrollingSpeed", speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = rBody.position;
        rBody.position += rBody.transform.right * -speed * Time.deltaTime;
        rBody.MovePosition(pos);
    }
}
