using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public struct PolarCoordinates
{

    public PolarCoordinates(float angle, float magnitude)
    {
        Angle = angle;
        Magnitude = magnitude;
    }

    public PolarCoordinates(Vector2 vec)
    {
        float angle = Vector2.SignedAngle(Vector2.right, vec);
        if (angle < 0) angle += 360;
        Angle = angle;
        Magnitude = vec.magnitude;
    }

    public float Angle { get; }
    public float Magnitude { get; }
    
    
}

