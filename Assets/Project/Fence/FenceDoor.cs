using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FenceDoor : MonoBehaviour
{
    public float animationDuration = 3f;
    private bool _opening = false;
    private Vector3 _endPosition;
    private float _timeForAnimation = 0f;
    private BoxCollider _collider;
    protected virtual void Start()
    {
        _endPosition = transform.position - Vector3.up * transform.lossyScale.y;
        _collider = GetComponent<BoxCollider>();
    }

    public void Open()
    {
        if(_opening) return;
        Destroy(_collider);
        _opening = true;
    }

    
    void Update()
    {
        if(!_opening) return;
        _timeForAnimation += Time.deltaTime;
        
        if (_timeForAnimation > animationDuration)
        {
            Destroy(gameObject);
            return;
        }
        
        transform.position = Vector3.Lerp(transform.position, _endPosition, _timeForAnimation / animationDuration);
    }
}
