using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sign : MonoBehaviour
{
    public TextBox textBox;
    
    private UnityEvent<Sign> _playerNear = new();
    private UnityEvent<Sign> _playerNotNear = new();
    
    void Start()
    {
        var player = FindObjectOfType<Player>();
        if (player == null)
        {
            throw new ArgumentException("player wasn't found in current scene");
        }
        if (textBox == null)
        {
            throw new ArgumentException("no text box was provided");
        }
        _playerNear.AddListener(player.OnNearSignEnter);
        _playerNotNear.AddListener(player.OnNearSignExit);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNear.Invoke(this);  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNotNear.Invoke(this);
        }
    }
}
