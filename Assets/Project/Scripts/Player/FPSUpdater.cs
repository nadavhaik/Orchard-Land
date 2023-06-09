using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSUpdater : MonoBehaviour
{
    public float updateInterval = 0.5f;
    private TextMeshProUGUI _fpsText;
    // private float _timeFromLastUpdate = 0;

    void UpdateFPS()
    {
        float dt = Time.deltaTime;
        if(dt != 0) 
            _fpsText.text = $"FPS: {Mathf.RoundToInt(1f / dt)}";
        Invoke(nameof(UpdateFPS), updateInterval);
    }

    private void Start()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
        Invoke(nameof(UpdateFPS), updateInterval);
    }
    
    
    
}
