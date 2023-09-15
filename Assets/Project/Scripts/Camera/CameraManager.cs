using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;
    public Camera initialCamera;
    public UnityEvent<Camera> onChangeCamera = new();
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
    void Start()
    {
        MainCamera = initialCamera;
    }
}
