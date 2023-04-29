using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Hittable
{

    protected override void InitHitHandlers()
    {
        SetHealthReducerHandler("Sword", 10f);
        SetHealthReducerHandler("Arrow", 5f);
    }

    protected override void Kill() => Destroy(gameObject);

}
