using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    // Start is called before the first frame update

    public bool debugControlPoints;
    public GameObject arrowStart;
    public GameObject arrowEnd;
    public GameObject sliders;
    public float minRotation = -90f;
    public float maxRotation = 90f;

    void Start()
    {
        GameObject[] controlPoints = { arrowStart, arrowEnd };
        foreach (var controlPoint in controlPoints)
        {
            controlPoint.GetComponent<Renderer>().enabled = debugControlPoints;
        }
    }

    public void Rotate(float angle)
    {
        float currentXRotation = -transform.rotation.eulerAngles.x % 360f;
        if (currentXRotation < -180f) currentXRotation += 360f;
        if (currentXRotation + angle > maxRotation || currentXRotation + angle < minRotation)  return;
        transform.Rotate(Vector3.right, -angle);
    }

    public void Shoot(Arrow arrow)
    {
        float tension = Vector2.Distance(arrow.transform.position, arrowStart.transform.position) /
                        Vector2.Distance(arrowEnd.transform.position, arrowStart.transform.position);
        arrow.Shoot(tension);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
