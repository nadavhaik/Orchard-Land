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
    private bool _cyclicTriggerFlag = false;
    private bool _inCoolDown = false;
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
            _cyclicTriggerFlag = false;
            otherSlider.OnOtherValueChanged(val);
            _player.SetArrowTension(val);
        });
        _player.arrowCooldownStartEvent.AddListener(() => _inCoolDown = true);
        _player.arrowCooldownEndEvent.AddListener(() => _inCoolDown = false);
    }
    
    private void OnOtherValueChanged(float newValue)
    {
        if(_cyclicTriggerFlag) return;
        _cyclicTriggerFlag = true;
        _slider.value = newValue;
        _cyclicTriggerFlag = false;
    }

    public void Reset()
    {
        _slider.value = 0;
        otherSlider.OnOtherValueChanged(0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _player.ShootBow();
        Reset();
    }
}
