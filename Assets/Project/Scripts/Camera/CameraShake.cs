using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private float _duration;
    private float _strength;

    private void Awake() => Instance = this;

    private void ShakeCamera()
    {
        transform.DOShakePosition(_duration, _strength);
        transform.DOShakeRotation(_duration, _strength);
    }

    private void OnShake(float duration, float strength, float delay)
    {
        _duration = duration;
        _strength = strength;
        Invoke(nameof(ShakeCamera), delay);
        
    }

    public static void Shake(float duration, float strength, float delay) => Instance.OnShake(duration, strength, delay);
}
