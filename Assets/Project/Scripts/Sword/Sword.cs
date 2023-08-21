using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum AttackDirection
{
    EastToWest,
    WestToEast,
    NorthToSouth,
    SouthToNorth,
    NeToSw,
    SwToNe,
    NwToSe,
    SeToNw
}


public abstract class Sword : MonoBehaviour
{
    public float swingDuration = 1.5f;
    public GameObject handle;
    public GameObject model;

    [Header("Control Points")] 
    public GameObject north;
    public GameObject south;
    public GameObject east;
    public GameObject west;
    public GameObject stabStart;
    public GameObject stabEnd;
    public GameObject backPosition;
    public bool drawControlPoints;
    

    private float _attackTimer;
    protected float cooldownTimer;
    protected float cooldown;
    private Quaternion _targetRotation;
    private Vector3 _targetPosition;
    private bool _attacking;
    private Vector3 _originalPos;
    private Quaternion _originalRot;
    protected AttackDirection attackDirectionAfterCooldown;


    protected Func<float, Vector3> movementCurve = _ => Vector3.zero;

    private Quaternion _attackStartRot;
    private Quaternion _attackEndRot;
    private BoxCollider _collider;
    protected MeleeWeaponTrail trail;

    
    protected AudioSource audioSource;

    protected abstract string activeSwordTag { get; }
    private const string NotActiveTag = "Untagged";


    public bool Attacking
    {
        get => _attacking;
        private set
        {
            _attacking = value;
            // _collider.enabled = _attacking;
            tag = _attacking ? activeSwordTag : NotActiveTag;
        }
    }
    
    public bool PreparingAttack
    {
        get => cooldown != 0;
    }
    

    protected virtual void Start()
    {
        _originalPos = transform.localPosition;
        _originalRot = transform.localRotation;
        _collider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        trail = GetComponent<MeleeWeaponTrail>();
        trail.Hide();
        cooldown = 0;
        cooldownTimer = 0;

        GameObject[] controlPoints = { north, south, east, west, stabStart, stabEnd };
        foreach (var controlPoint in controlPoints)
        {
            controlPoint.GetComponent<Renderer>().enabled = drawControlPoints;
        }

        Attacking = false;
    }
    

    public virtual void ResetPosition()
    {
        Attacking = false;
        trail.Hide();
        _targetRotation = _originalRot;
        _targetPosition = _originalPos;

        transform.localRotation = _targetRotation;
        transform.localPosition = _targetPosition;
    }

    public void PutOnBack()
    {
        Attacking = false;
        transform.position = backPosition.transform.position;
        transform.rotation = backPosition.transform.rotation;
    }

    public void Hold() => ResetPosition();

    // Update is called once per frame
    protected virtual void Update()
    {
        if (PreparingAttack)
        {
            transform.position = movementCurve(cooldownTimer / cooldown);
            cooldownTimer += Time.deltaTime;
        }
        if (Attacking)
        {
            transform.position = movementCurve(_attackTimer / swingDuration);
            transform.rotation = Quaternion.Lerp(_attackStartRot, _attackEndRot, _attackTimer / swingDuration);
            _attackTimer += Time.deltaTime;
        }

        
    }

    public bool CanAttack()
    {
        return !Attacking;
    }

    protected Func<float, Vector3> GetMovementCurve(Vector3 from, Vector3 to)
    {
        return t => Vector3.Lerp(from, to, t);
    }

    protected Func<float, Vector3> GetMovementCurve(AttackDirection direction)
    {
        return t =>
        {
            var northPos = north.transform.position;
            var southPos = south.transform.position;
            var eastPos = east.transform.position;
            var westPos = west.transform.position;

            var nePos = Vector3.Lerp(northPos, eastPos, 0.5f);
            var nwPos = Vector3.Lerp(northPos, westPos, 0.5f);
            var sePos = Vector3.Lerp(southPos, eastPos, 0.5f);
            var swPos = Vector3.Lerp(southPos, westPos, 0.5f);

            switch (direction)
            {
                case AttackDirection.NorthToSouth:
                    return Vector3.Lerp(northPos, southPos, t);
                case AttackDirection.SouthToNorth:
                    return Vector3.Lerp(southPos, northPos, t);
                case AttackDirection.EastToWest:
                    return Vector3.Lerp(eastPos, westPos, t);
                case AttackDirection.WestToEast:
                    return Vector3.Lerp(westPos, eastPos, t);
                case AttackDirection.NeToSw:
                    return Vector3.Lerp(nePos, swPos, t);
                case AttackDirection.SwToNe:
                    return Vector3.Lerp(swPos, nePos, t);
                case AttackDirection.NwToSe:
                    return Vector3.Lerp(nwPos, sePos, t);
                case AttackDirection.SeToNw:
                    return Vector3.Lerp(sePos, nwPos, t);
            }

            throw new ArgumentException("Illegal direction: " + direction);

        };
    }

    void RotateBeforeSwing(AttackDirection direction)
    {
        float zRotation = -1;
        switch (direction)
        {
            case AttackDirection.NorthToSouth:
                zRotation = 0;
                break;
            case AttackDirection.SouthToNorth:
                zRotation = 180;
                break;
            case AttackDirection.EastToWest:
                zRotation = 90;
                break;
            case AttackDirection.WestToEast:
                zRotation = 270;
                break;
            case AttackDirection.NeToSw:
                zRotation = 45;
                break;
            case AttackDirection.SwToNe:
                zRotation = 225;
                break;
            case AttackDirection.NwToSe:
                zRotation = 315;
                break;
            case AttackDirection.SeToNw:
                zRotation = 135;
                break;
            
        }
        
        
        transform.Rotate(0, 0, -zRotation);
    }
    
    public virtual void Swing(AttackDirection direction)
    {
        if(!CanAttack()) return;
        // trail.Draw();
        _attackTimer = 0f;
        movementCurve = GetMovementCurve(direction);
        var attackStartPosition = movementCurve(0);
        var attackEndPosition = movementCurve(1);
        RotateBeforeSwing(direction);


        transform.position = model.transform.position;
        // transform.LookAt(attackEndPosition);
        _attackEndRot = transform.rotation;
        
        // transform.LookAt(attackStartPosition);
        _attackStartRot = transform.rotation;
        transform.position = attackStartPosition;

        StartAttack();
    }

    private void StartAttack()
    {
        _attackTimer = 0f; // for animation interpolations only!
        trail.Draw();

        // resetting the logical timer:  
        CancelInvoke(nameof(ResetPosition));
        Invoke(nameof(ResetPosition), swingDuration);
        
        Attacking = true;
    }

    public void Stab()
    {
        if(!CanAttack()) return;
        
        
        movementCurve = t => Vector3.Lerp(stabStart.transform.position, stabEnd.transform.position, t);

        transform.position = movementCurve(0);
        transform.LookAt(movementCurve(1));
        _attackStartRot = _attackEndRot = transform.rotation;

        StartAttack();
    }
}
