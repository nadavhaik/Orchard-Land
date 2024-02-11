using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WheelPlank : MonoBehaviour
{
    public float spinSpeed = 720f;
    public GameObject rotAround;
    public bool refresh = false;
    private Collider _collider;
    private Vector3 _originalCenter;
    private Vector3 _originalPos;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        // Physics.IgnoreCollision(_collider, rotAround.GetComponent<BoxCollider>());
        _originalPos = transform.position;
    }

    private void Update()
    {
        // transform.position = new Vector3(_originalPos.x, transform.position.y, transform.position.z);
    }

    void FixedUpdate()
    {
        
        if (!IsSpinning)
        {
            if (refresh)
            {
                refresh = false;
                IsSpinning = true;
            }
            
            return;
        }
        transform.Rotate(Time.fixedDeltaTime * new Vector3(spinSpeed, 0f, 0f));
    }
    
    public bool IsSpinning { get; set; } = true;
}
