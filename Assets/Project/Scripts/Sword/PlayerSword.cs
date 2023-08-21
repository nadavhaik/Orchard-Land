using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : Sword
{
    
    [Header("Tagging")]
    public string ActiveSwordTag = "Sword";
    protected override string activeSwordTag { get => ActiveSwordTag; }

    public override void Swing(AttackDirection direction)
    {
        if(!CanAttack()) return;
        base.Swing(direction);
    }

    public override void ResetPosition()
    {
        base.ResetPosition();
    }
    //
    protected override void Start()
    {
        base.Start();
    }
    
    //
    // protected override void Update()
    // {
    //     base.Update();
    // }
}
