using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SwitchingCamera : MonoBehaviour
{
    public float speed = 5f;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Camera _dest;
    private Action _afterKilled;

    void Start()
    {
        // enabled = false;
    }


    public void Init(Camera start, Camera dest, Action afterKilled)
    {
        _startPosition = start.transform.position;
        _startRotation = start.transform.rotation;
        _dest = dest;
        _afterKilled = afterKilled;
        start.enabled = false;
        enabled = true;
        
        transform.position = _startPosition;
        transform.rotation = _startRotation;
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
        float totalDistance = Vector3.Distance(_startPosition, _dest.transform.position);
        float distancePerFrame = speed * totalDistance * Time.deltaTime;
        
        if(distanceFromDest <= distancePerFrame) Kill();
        
        var t = 1f - (distanceFromDest / totalDistance) + (distancePerFrame / totalDistance);
        
        transform.position = Vector3.Lerp(_startPosition, _dest.transform.position, t);
        transform.rotation = Quaternion.Lerp(_startRotation, _dest.transform.rotation, t);

    }
}
