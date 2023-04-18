using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    // Start is called before the first frame update
    public Arrow arrow;

    public bool debugControlPoints;
    public GameObject arrowStart;
    public GameObject arrowEnd;

    private Arrow _arrowInstance;
    void Start()
    {
        if (debugControlPoints)
        {
            arrowStart.GetComponent<Renderer>().enabled = true;
            arrowEnd.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            arrowStart.GetComponent<Renderer>().enabled = false;
            arrowEnd.GetComponent<Renderer>().enabled = false;
        }
        
    }

    void Shoot()
    {
        float tension = Vector2.Distance(_arrowInstance.transform.position, arrowEnd.transform.position) /
                        Vector2.Distance(arrowStart.transform.position, arrowEnd.transform.position);
        _arrowInstance.Shoot(tension);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
