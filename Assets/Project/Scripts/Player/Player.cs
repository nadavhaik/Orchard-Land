using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


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

public class Player : Hittable
{
    public HealthBar uiHealthBar;
    
    [Header("Visuals")] 
    public GameObject model;

    [Header("CameraControls")] 
    public MainCamera mainCamera;
    public Camera bowCamera;
    public SwitchingCamera switchingCamera;
    public float maxLockDistance = 5f;
    public float minUnlockDistance = 15f;
    
    [Header("Movement")]
    public float movementConstant = 5f;

    public float bowRotationConstant = 5f;
    public float jumpForce = 50f;

    [Header("Items and Equipment")]
    public PlayerItem defaultItem = PlayerItem.Bomb;
    public Sword sword;
    public Shield shield;
    public Bomb bomb;
    public Bow bow;
    public Arrow arrow;
    public float bombThrowForce = 50f;
    public float bombThrowingAngle = 30f;
    public float minBombPlaceForce = 0f;
    [FormerlySerializedAs("max0ombPlaceForce")] public float maxBombPlaceForce = 2f;
    public float arrowsCooldown = 0.2f;
    public float ignoreArrowSelfCollisionFor = 0.1f;


    [Header("Items Control Points")] 
    public bool drawControlPoints;
    public GameObject bombHold;
    public GameObject bombPut;
    public GameObject bowHold;
    public GameObject bowBack;
    
    
    
    [Header("Controls")] 
    public float minSwipeForSwing = 1f;
    public float maxDistForDoubleTap = 10f;
    public double maxTimeForDoubleTap = 0.30f;
    public float minAccForParry = 2.0f;
    public float parrySlowMotionDuration = 1.0f;
    public float regularMotionTimeScale = 1.0f;
    public float slowMotionTimeScale = 0.5f;
    

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
    private Arrow _prevArrowInstance;

    private bool _currentlySwitchingCameras;
    private Camera _mainCameraObj;
    
    private bool _cameraLocked;
    private GameObject _lockedOn;

    private bool _inSlowMotion = false;
    private AndroidJavaObject _accPlugin;

    protected override void UpdateHealthBar()
    {
        
    }

    public bool LockedOnATarget
    {
        get => _lockedOn != null;
    }
    
    protected override void AnimateHit() { }


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

    private Stack<Action> _cancelIgnoreSelfArrow = new();
    private void CancelIgnoreSelfArrow()
    {
        _cancelIgnoreSelfArrow.Pop()();
    }

    private void TemporaryIgnoreSelfArrowCollision(Arrow arrow1, Arrow arrow2)
    {
        var collider1 = arrow1.GetComponent<Collider>();
        var collider2 = arrow2.GetComponent<Collider>();
        
        Physics.IgnoreCollision(collider1, collider2, true);
        _cancelIgnoreSelfArrow.Push(() =>
            Physics.IgnoreCollision(collider1, collider2, false));
            
        Invoke(nameof(CancelIgnoreSelfArrow), ignoreArrowSelfCollisionFor);
    }

    void InitArrow()
    {
        Arrow newArrow = Instantiate(arrow);
        if (_prevArrowInstance != null)
        {
            TemporaryIgnoreSelfArrowCollision(_prevArrowInstance, newArrow);
        }
        _currentArrowInstance = newArrow;
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
        // _controls.PlayerNormal.Disable();
        switch (_currentItemEquipped)
        {
            case PlayerItem.None:
                return;
            case PlayerItem.Bomb:
                _controls.HoldingBomb.Enable();
                HoldBomb();
                break;
        }

        _controls.PlayerNormal.Disable();
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

    static bool IsLockable(Collider c)
    {
        return c.CompareTag("LockableEnemy");
    }


    void LockCamera()
    {
        // Debug.Log("Called LockCamera()");
        _cameraLocked = true;
        var currentPosition = transform.position;
        var colliders = Physics.OverlapSphere(currentPosition, maxLockDistance)
            .Where(collider => IsLockable(collider))
            .OrderBy(collider => Vector3.Distance(currentPosition, collider.transform.position));
        if (colliders.Any())
        {
            _lockedOn = colliders.First().gameObject;
        }
        shield.Defend();
        mainCamera.Lock();
    }

    void UnlockCamera()
    {
        _cameraLocked = false;
        _lockedOn = null;
        shield.PutOnBack();
        mainCamera.Unlock();
    }

    void InitLock(InputAction action)
    {
        action.performed += _ => LockCamera();
        action.canceled += _ => UnlockCamera();
    }



    bool CanParry()
    {
        return !sword.Attacking && shield.CurrentState == ShieldState.Defending;
    }

    void Parry()
    {
        if(CanParry()) shield.Parry();
    }

    public void StopSlowMotion()
    {
        Debug.Log("Stopped Slow motion!");
        _inSlowMotion = false;
        Time.timeScale = regularMotionTimeScale;
        Time.fixedDeltaTime /= slowMotionTimeScale;
    }
    public void EnterSlowMotion(float duration)
    {
        Debug.Log("Entered Slow motion!");
        _inSlowMotion = true;
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime *= slowMotionTimeScale;
        Invoke(nameof(StopSlowMotion), duration * slowMotionTimeScale);
    }

    public void HandleParry()
    {
        if(_inSlowMotion) return;
        Handheld.Vibrate();
        EnterSlowMotion(parrySlowMotionDuration);
    }

    void CheckAccForParry(Vector3 acc)
    {
        var accSize = acc.magnitude;
        if(accSize >= minAccForParry) Parry();
    }

    void InitNormalControls()
    {
        InitJump(_controls.PlayerNormal.Jump);
        InitMove(_controls.PlayerNormal.Move);
        InitRotate(_controls.PlayerNormal.Rotate);
       
// #if UNITY_ANDROID
//       _accPlugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
//       _accPlugin.Call("startSensorListening", "accelerometer");
// #endif   
    

        _controls.PlayerNormal.UseItem.performed += _ => PullItem();
        _controls.PlayerNormal.Motion.performed += ctx =>
        {
            var accVec = ctx.ReadValue<Vector3>();
            CheckAccForParry(accVec);
        };
        

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
            if(_currentlySwitchingCameras) return;
            _controls.PlayerNormal.Disable();
            _controls.LockCamera.Disable();
            _currentlySwitchingCameras = true;
            WieldBow();
            Instantiate(switchingCamera).Init(_mainCameraObj, bowCamera, 
                () => _currentlySwitchingCameras = false);
            _controls.BowPov.Enable();
            // mainCamera.enabled = false;
            // bowCamera.enabled = true;
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
        
        var oneOrMinusOne = 2f * (Mathf.Round(Random.Range(0, 1)) - 0.5f);
        var pushForce = new Vector3(
            Random.Range(minBombPlaceForce, maxBombPlaceForce) * oneOrMinusOne,
            0f,
            Random.Range(minBombPlaceForce, maxBombPlaceForce));
        _currentBombInstance.GetComponent<Rigidbody>().AddForce(model.transform.rotation * pushForce);
        
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
            
            _prevArrowInstance = _currentArrowInstance;
            _currentArrowInstance = null;
            Invoke( nameof(InitArrow), arrowsCooldown);
        };
        _controls.BowPov.Cancel.performed += _ =>
        {
            if(_currentlySwitchingCameras) return;
            _controls.BowPov.Disable();
            _currentlySwitchingCameras = true;
            
            if (_currentArrowInstance != null)
            {
                Destroy(_currentArrowInstance.gameObject);
            }
            
            mainCamera.ResetPosition();
            Instantiate(switchingCamera).Init(bowCamera, _mainCameraObj, 
                () =>
                {
                    _currentlySwitchingCameras = false;
                    _controls.LockCamera.LockCamera.Enable();
                });
            UnwieldBow();
            _controls.PlayerNormal.Enable();
        };
    }
    
    void Awake()
    {
        _controls = new Controls();
#if UNITY_ANDROID // Shame on you, Unity!
        Input.gyro.enabled = true;
#endif
        _currentItemEquipped = defaultItem;
        InitNormalControls();
        InitBombControls();
        InitBowPovControls();
        InitLock(_controls.LockCamera.LockCamera);
    }
    // Start is called before the first frame update


    void WieldBow()
    {
        bow.transform.position = bowHold.transform.position;
        bow.transform.rotation = bowHold.transform.rotation;
    }

    void UnwieldBow()
    {
        bow.transform.position = bowBack.transform.position;
        bow.transform.rotation = bowBack.transform.rotation;
    }

    protected override void InitHitHandlers()
    {
        SetHealthReducerHandler("SimpleEnemy", 10f);
        SetHealthReducerHandler("Explosion", 20f);
        SetHealthReducerHandler("EnemySword", 200f);
    }
    
    protected override void Start()
    {
        base.Start();
        healthBar = uiHealthBar;

        _playerRigidBody = GetComponent<Rigidbody>();
        _mainCameraObj = mainCamera.GetComponent<Camera>();
        
        var playerCollider = GetComponent<Collider>();
        var swordCollider = sword.GetComponent<Collider>();
        var shieldCollider = shield.GetComponent<Collider>();
        
        
        
        Physics.IgnoreCollision(playerCollider, shieldCollider);
        Physics.IgnoreCollision(swordCollider, shieldCollider);
        Physics.IgnoreCollision(playerCollider, swordCollider);
        
        
        GameObject[] controlPoints = { bombHold, bombPut, bowBack, bowHold };
        foreach (var controlPoint in controlPoints)
        {
            controlPoint.GetComponent<Renderer>().enabled = drawControlPoints;
        }
        
    }

    bool InPov() { return _controls.BowPov.enabled; }
    
    void MoveLocked(Vector2 movement)
    {
        transform.position += model.transform.rotation * new Vector3(movement.x, 0, movement.y) * (movementConstant * Time.deltaTime); // on the xz plane
    }

    void MoveTps(Vector2 movement)
    { 
        var deltaMovement = _mainCameraObj.transform.rotation * new Vector3(movement.x, 0, movement.y) * (movementConstant * Time.deltaTime); // on the xz plane
        var newPosition = transform.position + new Vector3(deltaMovement.x, 0, deltaMovement.z);
        model.transform.LookAt(newPosition);
        transform.position = newPosition;
        
    }

    void Move(Vector2 movement)
    {
        if(movement == Vector2.zero) return;
        if(InPov() || _cameraLocked) 
            MoveLocked(movement);
        else 
            MoveTps(movement);
    }

    void RotateBow(Vector2 movement)
    {
        float rotationConst = bowRotationConstant * Time.deltaTime;
        model.transform.Rotate(Vector3.up, rotationConst * movement.x);
        bow.Rotate(rotationConst * movement.y);
    }

    void RotateTps(Vector2 rotation)
    {
        mainCamera.Rotate(rotation);
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
    

    protected override void Kill()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Destroy(gameObject);
    }
    
    protected override void Update()
    {
        base.Update();
#if UNITY_ANDROID // Shame on you, Unity!
        if (_controls.PlayerNormal.Motion.enabled)
        {
            var acc = Input.acceleration;
            CheckAccForParry(acc);
        }
#endif
        
        CheckIfOnFloor();
        Move(_leftStick);
        Rotate(_rightStick);
        if (_lockedOn != null)
        {
            if (Vector3.Distance(transform.position, _lockedOn.transform.position) > minUnlockDistance)
            {
                UnlockCamera();
            }
            else
            {
                model.transform.LookAt(_lockedOn.transform.position);    
            }
        }
        
        if(_touching) _touchEnd = _touchPositionSource.ReadValue<Vector2>();
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
        _controls.LockCamera.Enable();
        _controls.BowPov.Disable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }


//     private void OnApplicationQuit()
//     {
// #if UNITY_ANDROID
//         if (_accPlugin != null)
//         {
//             _accPlugin.Call("terminate");
//             _accPlugin = null;
//         }
// #endif
//     }
}
