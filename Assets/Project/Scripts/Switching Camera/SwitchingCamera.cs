using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SwitchingCamera : MonoBehaviour
{
    public float speed = 5f;

    private Camera _start;
    private Camera _dest;
    private Action _afterKilled;

    void Start()
    {
        // enabled = false;
    }


    public void Init(Camera start, Camera dest, Action afterKilled)
    {
        _start = start;
        _dest = dest;
        _afterKilled = afterKilled;
        _start.enabled = false;
        enabled = true;
        
        transform.position = _start.transform.position;
        transform.rotation = _start.transform.rotation;
    }

    public void Init(Camera start, Camera dest)
    {
        Init(start, dest, () => {});
    }

    void Kill()
    {
        _dest.enabled = true;
        enabled = false;
        _afterKilled();
        Destroy(gameObject);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if(!enabled) return;

        var distanceFromDest = Vector3.Distance(transform.position, _dest.transform.position);
        float totalDistance = Vector3.Distance(_start.transform.position, _dest.transform.position);
        float distancePerFrame = speed * totalDistance * Time.deltaTime;
        
        if(distanceFromDest <= distancePerFrame) Kill();
        
        var t = 1f - (distanceFromDest / totalDistance) + (distancePerFrame / totalDistance);
        
        transform.position = Vector3.Lerp(_start.transform.position, _dest.transform.position, t);
        transform.rotation = Quaternion.Lerp(_start.transform.rotation, _dest.transform.rotation, t);

    }
}
