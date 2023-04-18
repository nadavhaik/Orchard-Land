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
    public float mass;
    
    // Start is called before the first frame update
    void Start()
    {
        _activated = false;
        _timer = 0f;
    }

    public void Activate()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.detectCollisions = true;
        rb.useGravity = true;
        rb.mass = mass;
        
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
