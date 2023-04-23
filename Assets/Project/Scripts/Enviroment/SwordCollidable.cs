using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SwordCollidable : MonoBehaviour
{
    // Start is called before the first frame update

    public float swordForce = 15f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.transform.CompareTag("Sword")) return;
        Vector3 collisionDirection = (transform.position - other.transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(swordForce * collisionDirection, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        
    }
}
