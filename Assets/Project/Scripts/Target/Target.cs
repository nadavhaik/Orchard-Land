using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public UnityEvent targetShotEvent;
    private bool _isActive = true; 

    protected void OnTriggerEnter(Collider other)
    {
       if(!_isActive || !other.CompareTag("Arrow")) return;
       _isActive = false;
       targetShotEvent.Invoke();
    }
    
}
