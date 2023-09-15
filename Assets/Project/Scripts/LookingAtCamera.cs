using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class LookingAtCamera : MonoBehaviour
{
    private CameraManager _cameraManager;

    private void Start()
    {
        _cameraManager = FindObjectOfType<CameraManager>();
        if (_cameraManager == null)
        {
            throw new ArgumentException("CameraManager wasn't found in current scene!");
        }
    }
    

    void Update()
    {
        transform.LookAt(_cameraManager.MainCamera.transform.position);
        // foreach (var camera in Camera.allCameras)
        // {
        //     if (camera.enabled)
        //     {
        //         transform.LookAt(camera.transform.position);
        //         break;
        //     }
        // }
    }
}
