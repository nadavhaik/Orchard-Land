using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Hittable
{
    public EnemyHealthBar healthBarPrefab;
    public float maxHealth;

    private static bool Is01(float a) {
        return a > 0 && a < 1;
    }
    public static bool PointInCameraView(Vector3 point, Camera camera)
    {
        Vector3 viewport = camera.WorldToViewportPoint(point);
        bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
        bool inFrontOfCamera = viewport.z > 0;
     
        RaycastHit depthCheck;
        bool objectBlockingPoint = false;
     
        Vector3 directionBetween = point - camera.transform.position;
        directionBetween = directionBetween.normalized;
     
        float distance = Vector3.Distance(camera.transform.position, point);
     
        if(Physics.Raycast(camera.transform.position, directionBetween, out depthCheck, distance + 0.05f)) {
            if(Vector3.Distance(point, depthCheck.point) > 0.5f) {
                objectBlockingPoint = true;
            }
        }
     
        return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
    }

    protected override void Start()
    {
        base.Start();
        if(healthBarPrefab == null) return;
        healthBarPrefab.maxHealth = maxHealth;
        healthBar = Instantiate(healthBarPrefab, transform);

        Debug.Log("instansiated");
    }
    

    protected override void UpdateHealthBar()
    {
        
    }


    protected override void InitHitHandlers()
    {
        SetHealthReducerHandler("Sword", 10f);
        SetHealthReducerHandler("Arrow", 5f);
    }

    protected override void Kill()
    {
        // Destroy(healthBar);
        Destroy(gameObject);
    }
}
