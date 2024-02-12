using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldState {OnBack, Defending, Parrying}

[RequireComponent(typeof(AudioSource))]
public class Shield : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject backPosition;
    public GameObject defendPosition;
    public GameObject parryEnd;
    public float parryDuration = 2f;
    public GameObject model;
    public GameObject gripPoint;
    public PlayerHands hands;

    public ShieldState CurrentState { get; private set; }
    private float _parryTimer;
    private Collider _collider;
    private AudioSource _audioSource;

    public void Parry()
    {
        if (CurrentState != ShieldState.Defending) Defend();
        _collider.enabled = true;
        tag = "ParryingShield";
        _parryTimer = 0f;
        CurrentState = ShieldState.Parrying;
    }

    public void PlayParrySfx() => _audioSource.Play();
    

    public void Defend()
    {
        _collider.enabled = true;
        tag = "DefendingShield";
        transform.position = defendPosition.transform.position;
        transform.rotation = defendPosition.transform.rotation;
        CurrentState = ShieldState.Defending;
    }

    public void PutOnBack()
    {
        _collider.enabled = false;
        tag = "Untagged";
        transform.position = backPosition.transform.position;
        transform.rotation = backPosition.transform.rotation;
        CurrentState = ShieldState.OnBack;
    }

    void Start()
    {
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _collider.enabled = false;
        PutOnBack();
    }

    // Update is called once per frame

    void Update()
    {
        if (CurrentState != ShieldState.Parrying) return;

        if (_parryTimer >= parryDuration) Defend();
        else
        {
            transform.position = Vector3.Lerp(defendPosition.transform.position, parryEnd.transform.position,
                _parryTimer / parryDuration);
            transform.rotation = Quaternion.Lerp(defendPosition.transform.rotation, parryEnd.transform.rotation,
                _parryTimer / parryDuration);
            _parryTimer += Time.deltaTime;
        }

    }
}
