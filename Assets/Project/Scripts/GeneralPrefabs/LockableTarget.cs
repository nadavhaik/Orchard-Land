using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockableTarget : MonoBehaviour
{
    public Image pointingArrowImage;
    public Canvas canvas;

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
    }

    public void Untarget()
    {
        pointingArrowImage.enabled = false;
    }
    
}
