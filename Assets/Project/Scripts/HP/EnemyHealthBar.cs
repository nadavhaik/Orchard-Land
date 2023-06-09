using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    private bool _shown;

    protected override void Redraw()
    {
        UpdateBar();
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.current.transform);
    }
}
