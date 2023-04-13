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
    

    private Vector3 _attackStartPosition;
    private Vector3 _attackEndPosition;
    private Vector3 _rotAxis;
    private float _rotAngle;
    
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

        transform.position = Vector3.Lerp( _attackStartPosition, _attackEndPosition,  _attackTimer / swingDuration);
        transform.rotation = Quaternion.Lerp(_attackStartRot, _attackEndRot, _attackTimer / swingDuration);
        _attackTimer += Time.deltaTime;
        if(_attackTimer >= swingDuration) ResetPosition();
    }

    bool CanAttack()
    {
        return true;
    }

    Tuple<Vector3, Vector3> GetDirectionalControlPoints(AttackDirection direction)
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
                return new Tuple<Vector3, Vector3>(northPos, southPos);
            case AttackDirection.SouthToNorth:
                return new Tuple<Vector3, Vector3>(southPos, northPos);
            case AttackDirection.EastToWest:
                return new Tuple<Vector3, Vector3>(eastPos, westPos);
            case AttackDirection.WestToEast:
                return new Tuple<Vector3, Vector3>(westPos, eastPos);
            case AttackDirection.NeToSw:
                return new Tuple<Vector3, Vector3>(nePos, swPos);
            case AttackDirection.SwToNe:
                return new Tuple<Vector3, Vector3>(swPos, nePos);
           case AttackDirection.NwToSe: 
               return new Tuple<Vector3, Vector3>(nwPos, sePos);
           case AttackDirection.SeToNw:
               return new Tuple<Vector3, Vector3>(sePos, nwPos);
        }

        throw new ArgumentException("Illegal direction: " + direction);
    }
    public void Swing(AttackDirection direction)
    {
        if(!CanAttack()) return;
        
        _attackTimer = 0f;
        var controlPoints = GetDirectionalControlPoints(direction);
        _attackStartPosition = controlPoints.Item1;
        _attackEndPosition = controlPoints.Item2;


        transform.position = model.transform.position;
        transform.LookAt(_attackEndPosition);
        _attackEndRot = transform.rotation;
        
        transform.LookAt(_attackStartPosition);
        _attackStartRot = transform.rotation;
        transform.position = _attackStartPosition;

        _attacking = true;
    }

    public void Stab()
    {
        if(!CanAttack()) return;
        
        _attackTimer = 0f;
        _attackStartPosition = stabStart.transform.position;
        _attackEndPosition = stabEnd.transform.position;

        transform.position = _attackStartPosition;
        transform.LookAt(_attackEndPosition);
        _attackStartRot = _attackEndRot = transform.rotation;

        _attacking = true;
    }
}
