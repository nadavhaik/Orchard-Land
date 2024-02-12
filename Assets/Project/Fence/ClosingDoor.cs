using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ClosingDoorState
{
    OPEN,
    CLOSED,
    CLOSING
}
[RequireComponent(typeof(BoxCollider))]
public class ClosingDoor : MonoBehaviour
{
    public float animationDuration = 3f;
    private ClosingDoorState _state = ClosingDoorState.OPEN;
    private Vector3 _endPosition;
    private float _timeForAnimation = 0f;
    private BoxCollider _collider;
    protected virtual void Start()
    {
        _endPosition = transform.position + Vector3.up * transform.lossyScale.y;
        _collider = GetComponent<BoxCollider>();
    }

    public void Close()
    {
        if(_state != ClosingDoorState.OPEN) return;
        _state = ClosingDoorState.CLOSING;
    }

    
    void Update()
    {
        if(_state != ClosingDoorState.CLOSING) return;
        _timeForAnimation += Time.deltaTime;
        
        if (_timeForAnimation > animationDuration)
        {
            _state = ClosingDoorState.CLOSED;
        }
        
        transform.position = Vector3.Lerp(transform.position, _endPosition, _timeForAnimation / animationDuration);
    }
}