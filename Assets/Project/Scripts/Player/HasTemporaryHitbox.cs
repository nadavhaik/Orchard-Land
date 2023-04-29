using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasTemporaryHitbox : MonoBehaviour
{
    public float hitboxLifetime = 0.5f;
    protected virtual void Start()
    {
        var hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider == null) 
            Debug.LogError("Component has no collider - and no hitbox to destroy!");
        else
            Destroy(hitboxCollider, hitboxLifetime);
    }
}
