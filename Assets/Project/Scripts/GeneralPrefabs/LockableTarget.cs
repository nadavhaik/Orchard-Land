using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LockableTarget : MonoBehaviour
{
    public Image pointingArrowImage;
    public Canvas canvas;
    public UnityEvent<LockableTarget> onTargeted = new();
    public UnityEvent<LockableTarget> onUntargeted = new();

    public void Start()
    {
        foreach (var camera in Camera.allCameras)
        {
            if (camera.enabled)
            {
                canvas.worldCamera = camera;
                break;
            }
        }
        Untarget();
    } 

    public void Target()
    {
        pointingArrowImage.enabled = true;
        onTargeted.Invoke(this);
    }

    public void Untarget()
    {
        pointingArrowImage.enabled = false;
        onUntargeted.Invoke(this);
    }
    
}
