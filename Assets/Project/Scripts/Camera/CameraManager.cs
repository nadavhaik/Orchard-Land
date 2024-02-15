using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;
    public Camera initialCamera;
    public UnityEvent<Camera> onChangeCamera = new();
    public static CameraManager Instance { get; private set; }
    public Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            return _mainCamera;
        }
        set
        {
            _mainCamera.enabled = false;
            value.enabled = true;
            _mainCamera = value;
            onChangeCamera.Invoke(value);
        }
    }

    private void Awake() => Instance = this;

    void Start()
    {
        MainCamera = initialCamera;
    }
}
