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
    private Camera _attachedCamera;
    private CameraManager _cameraManager;

    void Start()
    {
        _attachedCamera = GetComponent<Camera>();
        _cameraManager = FindObjectOfType<CameraManager>();
        if (_attachedCamera == null)
        {
            throw new ArgumentException("No attached camera was found!");
        }

        if (_cameraManager == null)
        {
            throw new ArgumentException("CameraManager wasn't found in current scene!");
        }
    }


    public void Init(Camera start, Camera dest, Action afterKilled)
    {
        _startPosition = start.transform.position;
        _startRotation = start.transform.rotation;
        _dest = dest;
        _afterKilled = afterKilled;

        _cameraManager.MainCamera = _attachedCamera;
        
        
        transform.position = _startPosition;
        transform.rotation = _startRotation;
    }

    public void Init(Camera start, Camera dest)
    {
        Init(start, dest, () => {});
    }

    void Kill()
    {
        _cameraManager.MainCamera = _dest;
        _afterKilled();
        Destroy(gameObject);
    }
    
    
    void Update()
    {
        if(!enabled) return;

        var distanceFromDest = Vector3.Distance(transform.position, _dest.transform.position);
        float totalDistance = Vector3.Distance(_startPosition, _dest.transform.position);
        float distancePerFrame = speed * totalDistance * Time.deltaTime;

        if (distanceFromDest <= distancePerFrame)
        {
            Kill();
            return;
        }
        
        var t = 1f - (distanceFromDest / totalDistance) + (distancePerFrame / totalDistance);
        
        transform.position = Vector3.Lerp(_startPosition, _dest.transform.position, t);
        transform.rotation = Quaternion.Lerp(_startRotation, _dest.transform.rotation, t);
    }
}
