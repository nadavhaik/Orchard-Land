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


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        Physics.IgnoreCollision(GetComponent<Collider>(), lockable.GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.transform.CompareTag("Sword")) return;
        Vector3 collisionDirection = (transform.position - other.transform.position).normalized;
        _rb.AddForce(swordForce * collisionDirection, ForceMode.Impulse);
        _audioSource.Play();
    }
    

    // Update is called once per frame
    void Update()
    {
  
    }
}
