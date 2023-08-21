using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySword : Sword
{
    [Header("Tagging")]
    public string ActiveSwordTag = "EnemySword";
    protected override string activeSwordTag { get => ActiveSwordTag; }
    
    public UnityEvent opponentDefendedEvent = new();
    public UnityEvent opponentParriedEvent = new();
    
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }
    
    private void HandleDefended()
    {
        opponentDefendedEvent.Invoke();
        ResetPosition();
        audioSource.Play();
    }

    private void HandleParried()
    {
        Debug.Log("Parried!");
        opponentParriedEvent.Invoke();
        ResetPosition();
        audioSource.Play();
    }
    
    void CheckTrigger(Collider other)
    {
        if(!Attacking) return;
        if (other.CompareTag("DefendingShield")) HandleDefended();
        else if (other.CompareTag("ParryingShield")) HandleParried();
    }

    private void OnTriggerStay(Collider other)
    {
        CheckTrigger(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckTrigger(other);
    }
    void SwingAfterCooldown()
    {
        cooldown = 0;
        Swing(attackDirectionAfterCooldown);
    }

    public void SwingPredictably(AttackDirection direction, float cooldown)
    {
        if(!CanAttack()) return;
        
        var cooldownStart = transform.position;

        movementCurve = GetMovementCurve(cooldownStart, GetMovementCurve(direction)(0));
        attackDirectionAfterCooldown = direction;
        this.cooldown = cooldown;
        cooldownTimer = 0f;
        
        Invoke(nameof(SwingAfterCooldown), cooldown);
    }
    
}
