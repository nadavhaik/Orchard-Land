using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BowSlider : MonoBehaviour, IPointerUpHandler
{
    private Slider _slider;
    public BowSlider otherSlider;
    private Player _player;
    
    private bool _inCoolDown = false;
    private float _currentValue = 0f;

    public float Value
    {
        get => _slider.value;
        set => _slider.value = value;
    }
    void Start()
    {
        _slider = GetComponent<Slider>();
        _player = FindObjectOfType<Player>();
        _slider.onValueChanged.AddListener(val =>
        {
            if (_inCoolDown)
            {
                _slider.value = 0;
                return;
            }
            otherSlider.Value = val;
            _player.ArrowTension = val;
        });
        _player.arrowCooldownStartEvent.AddListener(() => _inCoolDown = true);
        _player.arrowCooldownEndEvent.AddListener(() => _inCoolDown = false);
    }

    public void Reset()
    {
        _slider.value = 0;
        otherSlider.Value = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _player.ShootArrow();
        Reset();
    }
}
