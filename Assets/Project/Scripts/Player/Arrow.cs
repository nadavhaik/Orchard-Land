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
    private bool _active = false;
    private Vector3 _shootingDirection;
    public float fixFactor = 10f;
    void Start()
    {
        Destroy(gameObject, 15f);
    }

    void FixHead()
    {
        if(!_active || _rb.velocity == Vector3.zero) return;
        body.transform.forward =
            Vector3.Slerp(body.transform.forward, _rb.velocity.normalized, Time.deltaTime * fixFactor);
        // headRb.rotation = Quaternion.LookRotation(headRb.velocity);  
        // headRb.rotation = Quaternion.LookRotation(new Vector3(0, headRb.velocity.y, 0));  
    }

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FixHead();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"Collusion with {other.transform.tag}: {other.collider.gameObject}");
        if (other.transform.CompareTag("Floor"))
        {
            _active = false;    
        }
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
        _active = true;
        // GetComponent<Rigidbody>().AddForce(shootingForce * tension * transform.forward);
    }
}
