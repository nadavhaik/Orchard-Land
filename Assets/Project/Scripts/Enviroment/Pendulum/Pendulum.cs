using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : SwordCollidable
{
    // Start is called before the first frame update

    private Rigidbody _rb;
    private AudioSource _audioSource;
    public GameObject lockable;
    public float minTimeBetweenHits = 0.5f;
    private bool _hittable = true;

    void MarkHittable() => _hittable = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        Physics.IgnoreCollision(GetComponent<Collider>(), lockable.GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider other)
    {
       Hit(other);
    }
    
    private void OnTriggerStay(Collider other)
    {
        Hit(other);
    }

    void Hit(Collider other)
    {
        if(!_hittable || !other.transform.CompareTag("Sword")) return;
        
        _hittable = false;
        Vector3 collisionDirection = (transform.position - other.transform.position).normalized;
        _rb.AddForce(swordForce * collisionDirection, ForceMode.Impulse);
        _audioSource.Play();

        CancelInvoke(nameof(MarkHittable));
        Invoke(nameof(MarkHittable), minTimeBetweenHits);
    }
    
    
    

    // Update is called once per frame
    void Update()
    {
  
    }
}
