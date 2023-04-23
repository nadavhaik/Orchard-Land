using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float duration = 5f;
    public GameObject explosion;
    public float mass;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Activate()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.detectCollisions = true;
        rb.useGravity = true;
        rb.mass = mass;
        
        Invoke(nameof(Explode), duration);
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
        
    }

    private void OnParticleCollision(GameObject other)
    {
      if(other.CompareTag("Explosion")) Explode();
    }
}
