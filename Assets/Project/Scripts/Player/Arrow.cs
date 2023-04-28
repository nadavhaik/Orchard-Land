using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    public float shootingForce = 1000f;
    public float mass = 1f;
    public GameObject head;
    public GameObject body;
    private Rigidbody _rb;
    public float lifeTime = 15f;

    private bool _shouldSetNotActive;
    private bool _isActive;
    private const string NotActiveTag = "Untagged"; 
    private const string ActiveArrowTag = "Arrow";
    public bool IsActive
    {
        get => _isActive; 
        private set 
        {  
            _isActive = value;
            tag = _isActive ? ActiveArrowTag : NotActiveTag;
        } 
    }

    void Start()
    {
        IsActive = false;
    }
    
    
    // Interpolation method for FixHead(). Vector3.Lerp can also be used
    private static readonly Func<Vector3, Vector3, float, Vector3> RotationInterpolation = Vector3.Slerp;

    void FixHead()
    {
        if(!IsActive || _rb.velocity == Vector3.zero) return;
        float g = -Physics.gravity.y;
        body.transform.forward =
            RotationInterpolation(body.transform.forward, _rb.velocity.normalized, Time.deltaTime * g);
    }

    private void LateUpdate()
    {
        if (_shouldSetNotActive)
        {
            _shouldSetNotActive = false;
            IsActive = false;
        }
    }

    private void FixedUpdate()
    {
        FixHead();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        _shouldSetNotActive = true;
        // IsActive = false;
        // transform.parent = other.transform;
    }

    public void Shoot(float tension)
    {
        // transform.parent = transform.parent.parent.parent.parent;
        transform.parent = null;
        
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = true;
        _rb.detectCollisions = true;
        _rb.mass = mass;

        // rb.isKinematic = true;
        var shootingForce3d = shootingForce * tension * transform.forward;
        _rb.AddForce(shootingForce3d, ForceMode.Acceleration);
        
        IsActive = true;
        Destroy(gameObject, lifeTime);
        // GetComponent<Rigidbody>().AddForce(shootingForce * tension * transform.forward);
    }
}
