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

    [Header("CameraControls")] 
    public Camera mainCamera;

    public Camera bowCamera;
    
    [Header("Movement")]
    public float movementConstant = 5f;

    public float bowRotationConstant = 5f;
    public float jumpForce = 50f;

    [Header("Items and Equipment")]
    public PlayerItem defaultItem = PlayerItem.Bomb;
    public Sword sword;
    public Bomb bomb;
    public Bow bow;
    public Arrow arrow;
    public float bombThrowForce = 50f;
    public float bombThrowingAngle = 30f;
    public float arrowsCooldown = 0.5f;

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
    private InputAction _touchPositionSource;
    private Vector2 _leftStick;
    private Vector2 _rightStick;

    private Vector2 _touchStart;
    private Vector2 _touchEnd;
    private bool _touching;
    
    
    private bool _onFloor;
    private bool _isInvincable = false;
    private Rigidbody _playerRigidBody;
    
    private PlayerItem _currentItemEquipped;
    private Bomb _currentBombInstance;
    private Arrow _currentArrowInstance;
    
    
    
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
        action.performed += ctx => _leftStick = ctx.ReadValue<Vector2>();
        action.canceled += _ => _leftStick = Vector2.zero;
    }
    
    void InitRotate(InputAction action)
    {
        action.performed += ctx => _rightStick = ctx.ReadValue<Vector2>();
        action.canceled += _ => _rightStick = Vector2.zero;
    }

    void InitArrow()
    {
        _currentArrowInstance = Instantiate(arrow);
        _currentArrowInstance.transform.position = bow.arrowStart.transform.position;
        _currentArrowInstance.transform.rotation = bow.transform.rotation;
        _currentArrowInstance.transform.parent = bow.transform;
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

    void InitTouch(InputAction touchPress, InputAction touchPosition)
    {
        touchPress.performed += _ =>
        {
            _touchStart = _touchEnd = touchPosition.ReadValue<Vector2>();
            _touching = true;
            _touchPositionSource = touchPosition;
        };
        touchPosition.canceled += _ => _touching = false;
    }

    void InitNormalControls()
    {
        InitJump(_controls.PlayerNormal.Jump);
        InitMove(_controls.PlayerNormal.Move);

        _controls.PlayerNormal.UseItem.performed += _ => PullItem();

        InitTouch(_controls.PlayerNormal.TouchPress, _controls.PlayerNormal.TouchPosition);
        _controls.PlayerNormal.TouchPress.canceled += _ => Swing(_touchStart, _touchEnd);
        

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
        _controls.PlayerNormal.PullBow.performed += _ =>
        {
            _controls.PlayerNormal.Disable();
            mainCamera.enabled = false;
            _controls.BowPov.Enable();
            bowCamera.enabled = true;
            Invoke( nameof(InitArrow), arrowsCooldown);
        };
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
    

    void InitBowPovControls()
    {
        InitMove(_controls.BowPov.Move);
        InitRotate(_controls.BowPov.Rotate);
        InitTouch(_controls.BowPov.TouchPress, _controls.BowPov.TouchPosition);
        _controls.BowPov.ButtonShoot.performed += _ =>
        {
            if(_currentArrowInstance == null) return;
            _currentArrowInstance.transform.position = bow.arrowEnd.transform.position;
            // _currentBombInstance.transform.LookAt(bow.arrowStart.transform.position);
            bow.Shoot(_currentArrowInstance);
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(),
                _currentArrowInstance.GetComponent<Collider>());
            _currentArrowInstance = null;
            Invoke( nameof(InitArrow), arrowsCooldown);
        };
        _controls.BowPov.Cancel.performed += _ =>
        {
            if (_currentArrowInstance != null)
            {
                Destroy(_currentArrowInstance.gameObject);
            }
            bow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            _controls.BowPov.Disable();
            bowCamera.enabled = false;
            _controls.PlayerNormal.Enable();
            mainCamera.enabled = true;
        };
    }
    
    void Awake()
    {
        _controls = new Controls();
        _currentItemEquipped = defaultItem;
        InitNormalControls();
        InitBombControls();
        InitBowPovControls();
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

    bool InPov() { return _controls.BowPov.enabled; }

    // Update is called once per frame

    void MovePov(Vector2 movement)
    {
        transform.position += model.transform.rotation * new Vector3(movement.x, 0, movement.y) * (movementConstant * Time.deltaTime); // on the xz plane
    }

    void MoveTps(Vector2 movement)
    { 
        var deltaMovement = new Vector3(movement.x, 0, movement.y) * (movementConstant * Time.deltaTime); // on the xz plane
        var newPosition = transform.position + deltaMovement;
        model.transform.LookAt(newPosition);
        transform.position = newPosition;
        
    }

    void Move(Vector2 movement)
    {
        if(movement == Vector2.zero) return;
        if(InPov()) 
            MovePov(movement);
        else 
            MoveTps(movement);
    }

    void RotateBow(Vector2 movement)
    {
        float rotationConst = bowRotationConstant * Time.deltaTime;
        model.transform.Rotate(Vector3.up, rotationConst * movement.x);
        bow.Rotate(rotationConst * movement.y);
        
    }

    void RotateTps(Vector2 movement)
    {
        
    }

    void Rotate(Vector2 movement)
    {
        if(movement == Vector2.zero) return;
        if(_controls.BowPov.enabled) 
            RotateBow(movement);
        else
            RotateTps(movement);
    }

    void CheckIfOnFloor()
    {
        RaycastHit hit;
        _onFloor = Physics.Raycast(transform.position, Vector3.down, out hit, 1.01f);
    }
    
    void Update()
    {
        CheckIfOnFloor();
        Move(_leftStick);
        Rotate(_rightStick);
        
        if(_touching) _touchEnd = _touchPositionSource.ReadValue<Vector2>();
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
        _controls.BowPov.Disable();
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
