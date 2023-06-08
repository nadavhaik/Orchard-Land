using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    protected override void Redraw()
    {
        UpdateBar();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
