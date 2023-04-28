using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Hittable
{
    protected virtual void Start()
    {
        SetHealthReducerHandler("Sword", 10f);
        SetHealthReducerHandler("Arrow", 5f);
    }

    protected override void TryToHit(Collider other)
    {
        base.TryToHit(other);
        if(health <= 0) Destroy(gameObject);
    }
}
