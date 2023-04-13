using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Visuals")] 
    public GameObject model;
    
    [Header("Movement")]
    public float movementConstant = 5f;
    public float jumpForce = 50f;

    [Header("Equipment")] 
    public Sword sword;

    [Header("Controls")] 
    public float minSwipeForSwing = 1f;
    public float maxDistForDoubleTap = 10f;
    public double maxTimeForDoubleTap = 0.30f;

    private double _lastTapTime = 0;
    private Vector2 _lastTapPosition;
    public AttackDirection attackDirectionForTest;
    
    private Controls _controls;
    private Vector2 _move;

    private Vector2 _touchStart;
    private Vector2 _touchEnd;
    private bool _touching;
    
    
    private bool _onFloor;
    private Rigidbody _playerRigidBody;

    void Jump()
    {
        if(!_onFloor) return;
       _playerRigidBody.AddForce(0, jumpForce, 0);
    }
    void Awake()
    {
        _controls = new Controls();
        _controls.Player.Move.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _move = Vector2.zero;
        _controls.Player.Jump.performed += _ => Jump();
        _controls.Player.TouchPress.performed += _ =>
        {
            _touchStart = _touchEnd = _controls.Player.TouchPosition.ReadValue<Vector2>();
            _touching = true;
        };
        _controls.Player.TouchPress.canceled += _ =>
        {
            _touching = false;
            Swing(_touchStart, _touchEnd);
        };

        _controls.Player.TouchTap.performed += _ =>
        {
            double tapTime = Time.fixedTimeAsDouble;
            var tapPosition = _controls.Player.TouchPosition.ReadValue<Vector2>();
            if (tapTime - _lastTapTime > maxTimeForDoubleTap ||
                Vector2.Distance(ScaledToScreen(_lastTapPosition), ScaledToScreen(tapPosition)) > maxDistForDoubleTap)
                // double tap wasn't performed - values should be saved to check next time
            {
                Debug.Log($"dt from last tap is {tapTime - _lastTapTime}");
                Debug.Log($"distance from last tap is {Vector2.Distance(ScaledToScreen(_lastTapPosition), ScaledToScreen(tapPosition))}");
                _lastTapPosition = tapPosition;
                _lastTapTime = tapTime;
            }
            else
            { // double tap was performed
                _lastTapTime = 0;
                sword.Stab();
            }
        };
        _controls.Player.Attack.performed += _ => Swing(attackDirectionForTest);
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    void Move(Vector2 movement)
    {
        if(movement == Vector2.zero) return;
        var deltaMovement = new Vector3(movement.x, 0, movement.y) * (movementConstant * Time.deltaTime); // on the xz plane
        var newPosition = transform.position + deltaMovement;
        model.transform.LookAt(newPosition);
        transform.position = newPosition;
    }

    void CheckIfOnFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.01f))
        {
            _onFloor = true;
        }
        else
        {
            _onFloor = false;
        }
    }
    void Update()
    {
        CheckIfOnFloor();
        Move(_move);
        if(_touching) _touchEnd = _controls.Player.TouchPosition.ReadValue<Vector2>();
    }
    
    static readonly Dictionary<int, AttackDirection> InterpolatedDirections = new Dictionary<int, AttackDirection>{
            {0, AttackDirection.WestToEast},
            {45, AttackDirection.SwToNe},
            {90, AttackDirection.SouthToNorth},
            {135, AttackDirection.SeToNw},
            {180, AttackDirection.EastToWest},
            {225, AttackDirection.NeToSw},
            {270, AttackDirection.NorthToSouth},
            {315, AttackDirection.NwToSe}
        };

    // this method interpolates the attack direction for every angle ∈ [-360, 360]
    static AttackDirection GetDirection(float angle)
    {
        // 8 -> len(dict)
        // 45 -> 360 / len(dict)
        if (angle < 0) angle += 360f;
        int interpolatedAngle = (Mathf.RoundToInt((angle / 360f) * 8) * 45) % 360; // nearest 45° integer multiplier in [0, 360)
        if (!InterpolatedDirections.ContainsKey(interpolatedAngle))
        {
            throw new ArithmeticException("Got an unexpected angle: " + interpolatedAngle);
        }

        return InterpolatedDirections[interpolatedAngle];
    }

    static Vector2 ScaledToScreen(Vector2 screenPoint)
    {
        return 100f * new Vector2(screenPoint.x / Screen.width, screenPoint.y / Screen.height);
    }

    void Swing(Vector2 start, Vector2 end)

    {
        if(Vector2.Distance(start, end) < minSwipeForSwing) return;
        Vector2 scaledStart = ScaledToScreen(start);
        Vector2 scaledEnd = ScaledToScreen(end);
        
        var swingDirection = scaledEnd - scaledStart;
        float angle = Vector2.SignedAngle(Vector2.right, swingDirection);
        Swing(GetDirection(angle));
    }

    void Swing(AttackDirection direction)
    {
        sword.Swing(direction);
    }
    
    void Stab() {sword.Stab();}

    private void OnEnable()
    {
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Floor"))
    //     {
    //         _onFloor = true;
    //     }
    // }
    //
    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Floor"))
    //     {
    //         _onFloor = false;
    //     }
    // }
}
