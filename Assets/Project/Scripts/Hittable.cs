using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hittable : MonoBehaviour
{
    public float health;
    public float minTimeBetweenHits = 0.05f;
    protected double lastHitTime;
    protected Dictionary<string, Action> hitHandlers = new(); 
    private bool _hittable = true;
    
    void MarkHittable() => _hittable = true;

    protected void OnCollisionEnter(Collision other) => TryToHit(other.collider);
    // protected void OnCollisionStay(Collision other) => TryToHit(other.collider);

    protected void OnTriggerEnter(Collider other) => TryToHit(other);
    protected void OnTriggerStay(Collider other) => TryToHit(other);
    
    protected virtual void TryToHit(Collider other)
    {
        if(!_hittable || !hitHandlers.ContainsKey(other.tag)) return;
        CancelInvoke(nameof(MarkHittable));
        _hittable = false;
        hitHandlers[other.tag]();
        lastHitTime = Time.fixedTimeAsDouble;
        Invoke(nameof(MarkHittable), minTimeBetweenHits);
    }

    protected void AddHandler(string tag, Action handler)
    {
        hitHandlers[tag] = handler;
    }

    protected void SetHealthReducerHandler(string tag, float reduce)
    {
        AddHandler(tag, () =>
        {
            AnimateHit();
            health -= reduce;
        });
    }

    protected abstract void AnimateHit();
    
}
