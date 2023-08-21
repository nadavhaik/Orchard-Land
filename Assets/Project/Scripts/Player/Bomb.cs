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

    [Header("Grip Points")] public GameObject leftGripPoint;
    public GameObject rightGripPoint;

    private AudioSource _audioSource;

    private bool AttachedToHands 
    {
        get
        {
            var leftHand = GameObject.Find("Left Hand");
            var rightHand = GameObject.Find("Right Hand");

            return (leftHand != null && leftHand.transform.parent == transform) ||
                   (rightHand != null && rightHand.transform.parent == transform);
        }
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        var allBombs = FindObjectsOfType<Bomb>();
        var myCollider = GetComponent<Collider>();
        foreach (var bomb in allBombs)
        {
            if(bomb == this) continue;
            Physics.IgnoreCollision(myCollider, bomb.GetComponent<Collider>());
        }
    }

    public void Activate()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.detectCollisions = true;
        rb.useGravity = true;
        rb.mass = mass;
        
        Invoke(nameof(Explode), duration);
        _audioSource.Play();
    }

    void Explode()
    {
        if(AttachedToHands) return; // preventing hands from being destroyed with the bomb
        Instantiate(explosion, gameObject.transform.position, 
            explosion.transform.rotation);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Explosion")) Explode();
    }

}
