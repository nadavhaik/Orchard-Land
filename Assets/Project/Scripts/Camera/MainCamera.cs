using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCamera : MonoBehaviour
{


    public GameObject model;

    public float distanceFromPlayer = 10f;
    [FormerlySerializedAs("height")] public float startHeight = 3f;
    public float rotationSpeed = 0.1f;
    public float heightChangeSpeed = 10f;
    public float minHeight = 0.5f;
    public float maxHeight = 5f;
    // public float startAngle = 180f;
    public float lockSpeed = 10f;
    
    private float _xzAngle;
    private float _yOffset;
    private bool _locked;

    private float Height
    {
        get => startHeight + _yOffset;
        set
        {
            if(value < minHeight || value > maxHeight) return;
            _yOffset = value - startHeight;
        }
    }


    // Start is called before the first frame update

    float StartAngle
    {
        get
        {
            var forward = model.transform.forward;
            var forward2d = new Vector2(forward.x, forward.z);
            var angle = Vector2.SignedAngle(Vector2.left, forward2d) % 360f;
            if (angle < -180f) angle += 180f;
            return angle;
        }
    }

    void Start()
    {
        ResetPosition();
    }

    public void ResetPosition()
    {
        _xzAngle = StartAngle;
        Height = startHeight;
        CalculatePosition();
    }
    

    void CalculatePosition()
    {
        var modelPos = model.transform.position;
        var angleRadians = _xzAngle * Mathf.PI / 180f;
        Vector3 positionOffset = new Vector3(
            Mathf.Cos(angleRadians) * distanceFromPlayer,
            Height,
            Mathf.Sin(angleRadians) * distanceFromPlayer);
        transform.position = modelPos + positionOffset;
        transform.LookAt(modelPos);
    }

    public void Rotate(Vector2 rotation)
    {
        if(_locked) return;
        var rotationVec = Time.deltaTime * rotation;
        _xzAngle -= rotationSpeed * rotationVec.x;
        Height += heightChangeSpeed * rotationVec.y;
        CalculatePosition();
    }

    public void Lock()
    {
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
    }

    // Update is called once per frame

    void Update()
    {
        if(!_locked || Mathf.Approximately(StartAngle, _xzAngle)) return;
        var t = Time.deltaTime * lockSpeed;
        _xzAngle = Mathf.LerpAngle(_xzAngle, StartAngle, t);
        Height = Mathf.Lerp(Height, startHeight, t);
        CalculatePosition();
    }
}
