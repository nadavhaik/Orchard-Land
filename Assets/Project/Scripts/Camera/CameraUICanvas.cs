using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUICanvas : MonoBehaviour
{
    private Canvas _canvas;
    
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null)
        {
            throw new ArgumentException("No attached canvas was found!");
        }

        var cameraManager = FindObjectOfType<CameraManager>();
        if (cameraManager != null)
        {
            cameraManager.onChangeCamera.AddListener(ChangeCamera);
        }
    }
    private void ChangeCamera(Camera newCamera) => _canvas.worldCamera = newCamera;
}
