using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class LookingAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        foreach (var camera in Camera.allCameras)
        {
            if (camera.enabled)
            {
                transform.LookAt(camera.transform.position);
                break;
            }
        }
    }
}
