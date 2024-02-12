using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollidableTrigger : MonoBehaviour
{
    public UnityEvent onTouch;
    private Collider _collider;
    private bool _triggered = false;
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        _triggered = true;
        onTouch.Invoke();
    }
}
