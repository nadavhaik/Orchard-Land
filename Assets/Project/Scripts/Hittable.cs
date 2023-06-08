using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hittable : MonoBehaviour
{
    protected HealthBar healthBar;
    
    public float minTimeBetweenHits = 0.05f;
    protected double lastHitTime;
    private Dictionary<string, Action> _hitHandlers = new(); 
    private bool _hittable = true;
    
    void MarkHittable() => _hittable = true;

    protected void OnCollisionEnter(Collision other) => TryToHit(other.collider);
    // protected void OnCollisionStay(Collision other) => TryToHit(other.collider);

    protected void OnTriggerEnter(Collider other) => TryToHit(other);
    protected void OnTriggerStay(Collider other) => TryToHit(other);

    protected abstract void InitHitHandlers();

    protected virtual void Start()
    {
        InitHitHandlers();
    }

    private void TryToHit(Collider other)
    {
        if(!_hittable || !_hitHandlers.ContainsKey(other.tag)) return;
        CancelInvoke(nameof(MarkHittable));
        _hittable = false;
        _hitHandlers[other.tag]();
        lastHitTime = Time.fixedTimeAsDouble;
        Invoke(nameof(MarkHittable), minTimeBetweenHits);
    }

    protected void SetHandler(string tag, Action handler)
    {
        _hitHandlers[tag] = handler;
    }

    protected abstract void UpdateHealthBar();

    protected void SetHealthReducerHandler(string tag, float reduce)
    {
        SetHandler(tag, () =>
        {
            AnimateHit();
            if(healthBar == null) return;
            healthBar.Reduce(reduce);
            UpdateHealthBar();
        });
    }

    protected virtual void Update()
    {
        if(healthBar == null) return;
        if (healthBar.CurrentHealth <= 0)
        {
            // Destroy(healthBar);
            Debug.Log($"Killing {gameObject.tag}");
            Kill();
        }
    }

    protected abstract void AnimateHit();
    protected abstract void Kill();

}
