using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MyMath
{
    public static bool ApproximatelyEqual(Vector3 v1, Vector3 v2)
    {
        return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y)
                                               && Mathf.Approximately(v1.z, v2.z);
    }
    
    public static bool ApproximatelyEqual(Vector2 v1, Vector2 v2)
    {
        return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);
    }
    
    public static bool ApproximatelyEqual(Quaternion q1, Quaternion q2)
    {
        return Mathf.Approximately(q1.x, q2.x) && Mathf.Approximately(q1.y, q2.y) &&
               Mathf.Approximately(q1.z, q2.z) && Mathf.Approximately(q1.w, q2.w);
    }
    
    
}
