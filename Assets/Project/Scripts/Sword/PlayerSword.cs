using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : Sword
{
    
    [Header("Tagging")]
    public string ActiveSwordTag = "Sword";
    protected override string activeSwordTag { get => ActiveSwordTag; }
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
