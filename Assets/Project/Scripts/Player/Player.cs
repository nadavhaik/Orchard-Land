using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public enum PlayerItem
{
    None,
    Bomb
};

public enum ControlScheme
{
    Normal,
    HoldingBomb
}

public class Player : MonoBehaviour
{
    [Header("Visuals")] 
    public GameObject model;
    
    [Header("Movement")]
    public float movementConstant = 5f;
    public float jumpForce = 50f;

    [Header("Items and Equipment")]
    public PlayerItem defaultItem = PlayerItem.Bomb;
    public Sword sword;
    public Bomb bomb;
    public float bombThrowForce = 50f;
    public float bombThrowingAngle = 30f;

    [Header("Items Control Points")] 
    public bool drawControlPoints;
    public GameObject bombHold;
    public GameObject bombPut;
    
    
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
    private bool _isInvincable = false;
    private Rigidbody _playerRigidBody;
    
    private PlayerItem _currentItemEquipped;
    private Bomb _currentBombInstance;
    
    
    
    void Jump()
    {
        if(!_onFloor) return;
       _playerRigidBody.AddForce(0, jumpForce, 0);
    }

    void InitJump(InputAction action)
    {
        action.performed += _ => Jump();
    }

    void InitMove(InputAction action)
    {
        action.performed += ctx => _move = ctx.ReadValue<Vector2>();
        action.canceled += _ => _move = Vector2.zero;
    }

    void HoldBomb()
    {
        _currentBombInstance = Instantiate(bomb, gameObject.transform);
        _currentBombInstance.transform.position = bombHold.transform.position;
    }
    
    void PullItem()
    {
        _controls.PlayerNormal.Disable();
        switch (_currentItemEquipped)
        {
            case PlayerItem.None:
                _controls.PlayerNormal.Enable();
                return;
            case PlayerItem.Bomb:
                _controls.HoldingBomb.Enable();
                HoldBomb();
                return;
        }
    }

    void InitNormalControls()
    {
        InitJump(_controls.PlayerNormal.Jump);
        InitMove(_controls.PlayerNormal.Move);

        _controls.PlayerNormal.UseItem.performed += _ => PullItem();
        
        _controls.PlayerNormal.TouchPress.performed += _ =>
        {
            _touchStart = _touchEnd = _controls.PlayerNormal.TouchPosition.ReadValue<Vector2>();
            _touching = true;
        };
        _controls.PlayerNormal.TouchPress.canceled += _ =>
        {
            _touching = false;
            Swing(_touchStart, _touchEnd);
        };

        _controls.PlayerNormal.TouchTap.performed += _ =>
        {
            double tapTime = Time.fixedTimeAsDouble;
            var tapPosition = _controls.PlayerNormal.TouchPosition.ReadValue<Vector2>();
            if (tapTime - _lastTapTime > maxTimeForDoubleTap ||
                Vector2.Distance(ScaledPixel(_lastTapPosition), ScaledPixel(tapPosition)) > maxDistForDoubleTap)
                // double tap wasn't performed - values should be saved to check next time
            {
                _lastTapPosition = tapPosition;
                _lastTapTime = tapTime;
            }
            else
            { // double tap was performed
                _lastTapTime = 0;
                Stab();
            }
        };
        _controls.PlayerNormal.Attack.performed += _ => sword.Swing(attackDirectionForTest);
    }


    void CancelBomb()
    {
        Destroy(_currentBombInstance.gameObject);
        _controls.HoldingBomb.Disable();
        _controls.PlayerNormal.Enable();
    }

    void ThrowBomb()
    {
        
        
        _currentBombInstance.transform.parent = transform.parent;
        _currentBombInstance.Activate();
        
        var throwingDirection = (Quaternion.AngleAxis(
            -bombThrowingAngle, model.transform.right) * model.transform.forward).normalized;
        var throwForce3d = bombThrowForce * throwingDirection; 
        _currentBombInstance.GetComponent<Rigidbody>().AddForce(throwForce3d);
        
        _controls.HoldingBomb.Disable();
        _controls.PlayerNormal.Enable();
    }

    void PutBomb()
    {
        _currentBombInstance.transform.position = bombPut.transform.position;
        _currentBombInstance.transform.parent = transform.parent;
        _currentBombInstance.Activate();
        
        _controls.HoldingBomb.Disable();
        _controls.PlayerNormal.Enable();
    }
    void InitBombControls()
    {
        InitMove(_controls.HoldingBomb.Move);
        InitJump(_controls.HoldingBomb.Jump);

        _controls.HoldingBomb.Put.performed += _ => PutBomb();
        _controls.HoldingBomb.Cancel.performed += _ => CancelBomb();
        _controls.HoldingBomb.Throw.performed += _ => ThrowBomb();
    }
    
    void Awake()
    {
        _controls = new Controls();
        _currentItemEquipped = defaultItem;
        InitNormalControls();
        InitBombControls();
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
        if (drawControlPoints)
        {
            bombHold.GetComponent<Renderer>().enabled = true;
            bombPut.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            bombHold.GetComponent<Renderer>().enabled = false;
            bombPut.GetComponent<Renderer>().enabled = false;
        }
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
        _onFloor = Physics.Raycast(transform.position, Vector3.down, out hit, 1.01f);
    }
    
    void Update()
    {
        CheckIfOnFloor();
        Move(_move);
        if(_touching) _touchEnd = _controls.PlayerNormal.TouchPosition.ReadValue<Vector2>();
        // if (_controls.HoldingBomb.enabled)
        // {
        //     _currentBombInstance.transform.position = bombHold.transform.position;
        // }
    }

    void Hit()
    {
        
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
        int interpolatedAngle = (Mathf.RoundToInt((angle / 360f) * 8) * 45) % 360; // nearest 45° integer multiplier in [-180, 180)
        if (!InterpolatedDirections.ContainsKey(interpolatedAngle))
        {
            throw new ArithmeticException("Got an unexpected angle: " + interpolatedAngle);
        }

        return InterpolatedDirections[interpolatedAngle];
    }

    static Vector2 ScaledPixel(Vector2 onScreenPixel)
    {
        return 100f * new Vector2(onScreenPixel.x / Screen.width, onScreenPixel.y / Screen.height);
    }

    void Swing(Vector2 start, Vector2 end)

    {
        if(!_onFloor || Vector2.Distance(start, end) < minSwipeForSwing) return;
        Vector2 scaledStart = ScaledPixel(start);
        Vector2 scaledEnd = ScaledPixel(end);
        
        var swingDirection = scaledEnd - scaledStart;
        float angle = Vector2.SignedAngle(Vector2.right, swingDirection);
        sword.Swing(GetDirection(angle));
    }

    void Stab()
    {
        if(!_onFloor) return;
        sword.Stab();
    }

    private void OnEnable()
    {
        _controls.PlayerNormal.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"Collusion with particle {other.tag}");
    }
}
