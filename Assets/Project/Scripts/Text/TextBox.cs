using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TextBoxState
{
    Hidden,
    Showing,
    Shown,
    Hiding
}
public class TextBox : MonoBehaviour
{
    public GameObject panel;
    public float animationDuration = 0.5f;
    public float minScaleForAnimation = 0.1f;
    public float maxScaleForAnimation = 1f;
    
    private TextBoxState _currentState;
    private float _animationTimer;
    private float ScaleSpeed => (maxScaleForAnimation - minScaleForAnimation) / animationDuration;
    private float CurrentScale => panel.transform.localScale.x;
    
    private void ScalePanel(float newScale) =>
        panel.transform.localScale = new Vector3(newScale, newScale, 1f);
    
        

    public void Start()
    {
        if (panel == null)
        {
            throw new ArgumentException("null panel provided!");
        }
        
        _currentState = TextBoxState.Hidden;
        _animationTimer = 0f;
        ScalePanel(minScaleForAnimation);
        panel.SetActive(false);
    }

    public void Show()
    {
        switch (_currentState)
        {
            case TextBoxState.Shown or TextBoxState.Showing:
                return;
            case TextBoxState.Hidden:
                panel.SetActive(true);
                break;
        }

        _currentState = TextBoxState.Showing;
    }

    public void Hide()
    {
        if(_currentState is TextBoxState.Hidden or TextBoxState.Hiding) return;
        _currentState = TextBoxState.Hiding;
    }


    private void AnimateShow()
    {
        var newScale = CurrentScale + Time.deltaTime * ScaleSpeed;
        if (newScale >= maxScaleForAnimation)
        {
            newScale = maxScaleForAnimation;
            _currentState = TextBoxState.Shown;
        }
        ScalePanel(newScale);
    }

    private void AnimateHide()
    {
        var newScale = CurrentScale - Time.deltaTime * ScaleSpeed;
        if (newScale <= minScaleForAnimation)
        {
            newScale = minScaleForAnimation;
            _currentState = TextBoxState.Hidden;
        }
        
        ScalePanel(newScale);
        if (_currentState == TextBoxState.Hidden)
            panel.SetActive(false);
        
    }

    private void Update()
    {
        if(_currentState == TextBoxState.Showing)
            AnimateShow();
        else if(_currentState == TextBoxState.Hiding) 
            AnimateHide();
    }
}
