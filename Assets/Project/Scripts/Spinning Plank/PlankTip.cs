using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankTip : MonoBehaviour
{
    public WheelPlank plank;
    public float returnForce = 50f;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ParryingShield"))
        {
            plank.Stop();
            _rigidbody.AddForce(new Vector3(0, 0, returnForce));
        }
            
    }
}
