using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    public float shootingForce = 1000f;
    private float _originalMass;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _originalMass = _rb.mass;
        _rb.mass = 0;
        _rb.useGravity = false;
        _rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(float tension)
    {
        _rb.transform.parent = _rb.transform.parent.parent.parent;
        _rb.useGravity = true;
        _rb.mass = _originalMass;
        _rb.isKinematic = true;
        _rb.AddForce(shootingForce * tension * transform.forward);
    }
}
