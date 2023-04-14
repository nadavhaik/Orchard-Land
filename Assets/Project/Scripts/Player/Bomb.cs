using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float duration = 5f;
    private bool _activated;
    private float _timer;
    public GameObject explosion;
    private float _originalMass;
    
    // Start is called before the first frame update
    void Start()
    {
        _activated = false;
        _timer = 0f;
        _originalMass = GetComponent<Rigidbody>().mass;
        GetComponent<Rigidbody>().detectCollisions = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().mass = 0;
    }

    public void Activate()
    {
        GetComponent<Rigidbody>().detectCollisions = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().mass = _originalMass;
        _activated = true;
    }

    void Explode()
    {
        Instantiate(explosion, gameObject.transform.position, 
            explosion.transform.rotation);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_activated) _timer += Time.deltaTime;
        if(_timer > duration) Explode();
        // if(_exploded && _explosionInstance == null) Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
      if(other.CompareTag("Explosion")) Explode();
    }
}
