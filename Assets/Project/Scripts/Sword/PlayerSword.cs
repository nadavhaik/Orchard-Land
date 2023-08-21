using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : Sword
{
    
    [Header("Tagging")]
    public string ActiveSwordTag = "Sword";
    protected override string activeSwordTag { get => ActiveSwordTag; }
    
    // protected override void Start()
    // {
    //     base.Start();
    // }
    //
    // protected override void Update()
    // {
    //     base.Update();
    // }
}
