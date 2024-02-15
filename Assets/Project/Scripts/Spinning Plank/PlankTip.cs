using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlankTip : MonoBehaviour
{
    [FormerlySerializedAs("plank")] public WheelPlank wheelPlank;
    public GameObject plankObj;
    public float returnForce = 50f;
    private Rigidbody _rigidBody;
    public UnityEvent onParriedEvent;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.rotation = plankObj.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ParryingShield") || !wheelPlank.IsSpinning) return;
        
        wheelPlank.IsSpinning = false;
        onParriedEvent.Invoke();
        _rigidBody.AddForce(new Vector3(0, 0, returnForce));
    }
}
