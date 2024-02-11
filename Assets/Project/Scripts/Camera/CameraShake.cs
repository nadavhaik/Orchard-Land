using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private void Awake() => Instance = this;

    private void OnShake(float duration, float strength)
    {
        transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
    }

    public static void Shake(float duration, float strength) => Instance.OnShake(duration, strength);
}
