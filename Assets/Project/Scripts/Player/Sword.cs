using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


public class Sword : MonoBehaviour
{
    public float swingDuration = 1.5f;
    public GameObject handle;
    public GameObject model;

    [Header("Attack Control Points")] 
    public GameObject north;
    public GameObject south;
    public GameObject east;
    public GameObject west;
    public GameObject stabStart;
    public GameObject stabEnd;
    public bool drawControlPoints;

    private float _attackTimer;
    private Quaternion _targetRotation;
    private Vector3 _targetPosition;
    private bool _attacking;
    private Vector3 _originalPos;
    private Quaternion _originalRot;


    private Func<float, Vector3> _movementCurve = _ => Vector3.zero;

    private Quaternion _attackStartRot;
    private Quaternion _attackEndRot;
    

    void Start()
    {
        _originalPos = transform.localPosition;
        _originalRot = transform.localRotation;
        
        if (drawControlPoints)
        {
            north.GetComponent<Renderer>().enabled = true;
            south.GetComponent<Renderer>().enabled = true;
            east.GetComponent<Renderer>().enabled = true;
            west.GetComponent<Renderer>().enabled = true;
            stabStart.GetComponent<Renderer>().enabled = true;
            stabEnd.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            north.GetComponent<Renderer>().enabled = false;
            south.GetComponent<Renderer>().enabled = false;
            east.GetComponent<Renderer>().enabled = false;
            west.GetComponent<Renderer>().enabled = false;
            stabStart.GetComponent<Renderer>().enabled = false;
            stabEnd.GetComponent<Renderer>().enabled = false;
        }
    }

    void ResetPosition()
    {
        _attacking = false;
        _targetRotation = _originalRot;
        _targetPosition = _originalPos;

        transform.localRotation = _targetRotation;
        transform.localPosition = _targetPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_attacking) return;

        transform.position = _movementCurve(_attackTimer / swingDuration);
        transform.rotation = Quaternion.Lerp(_attackStartRot, _attackEndRot, _attackTimer / swingDuration);
        _attackTimer += Time.deltaTime;
        if(_attackTimer >= swingDuration) ResetPosition();
    }

    bool CanAttack()
    {
        return true;
    }

    Func<float, Vector3> GetMovementCurve(AttackDirection direction)
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
    public void Swing(AttackDirection direction)
    {
        if(!CanAttack()) return;
        
        _attackTimer = 0f;
        _movementCurve = GetMovementCurve(direction);
        var attackStartPosition = _movementCurve(0);
        var attackEndPosition = _movementCurve(1);


        transform.position = model.transform.position;
        transform.LookAt(attackEndPosition);
        _attackEndRot = transform.rotation;
        
        transform.LookAt(attackStartPosition);
        _attackStartRot = transform.rotation;
        transform.position = attackStartPosition;

        _attacking = true;
    }

    public void Stab()
    {
        if(!CanAttack()) return;
        
        _attackTimer = 0f;
        _movementCurve = t => Vector3.Lerp(stabStart.transform.position, stabEnd.transform.position, t);

        transform.position = _movementCurve(0);
        transform.LookAt(_movementCurve(1));
        _attackStartRot = _attackEndRot = transform.rotation;

        _attacking = true;
    }
}
