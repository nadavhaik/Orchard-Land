using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public Sword sword;
    public Shield shield;

    private PositionRotation _leftOrigin;
    private PositionRotation _rightOrigin;
    
    public bool InDefensivePosition { get => shield.CurrentState == ShieldState.Defending; }
    
    void Start()
    {
        _leftOrigin = PositionRotation.LocalFromTransform(leftHand.transform);
        _rightOrigin = PositionRotation.LocalFromTransform(rightHand.transform);
    }

    void ResetLeftHand()
    {
        leftHand.transform.parent = sword.transform;
        leftHand.transform.localPosition = _leftOrigin.Position;
        leftHand.transform.localRotation = _leftOrigin.Rotation;
    }

    void ResetRightHand()
    {
        rightHand.transform.parent = transform;
        rightHand.transform.localPosition = _rightOrigin.Position;
        rightHand.transform.localRotation = _rightOrigin.Rotation;
    }

    void HoldSword()
    {
        sword.ResetPosition();
        ResetLeftHand();
    } 

    void HoldShield()
    {
        rightHand.transform.position = shield.gripPoint.transform.position;
        rightHand.transform.parent = shield.transform;
    }

    public void DefensivePosition()
    {
        HoldShield();
        shield.Defend();
    }

    public void PutShieldOnBack()
    {
        shield.PutOnBack();
        ReleaseShield();
    }

    void ReleaseShield() => ResetRightHand();

    public void HoldBomb(Bomb bomb)
    {
        sword.PutOnBack();
        PutShieldOnBack();
        leftHand.transform.position = bomb.leftGripPoint.transform.position;
        rightHand.transform.position = bomb.rightGripPoint.transform.position;
        leftHand.transform.parent = rightHand.transform.parent = bomb.transform;
    }

    public void ReleaseBomb()
    {
        HoldSword();
        ResetRightHand();
    }

}
