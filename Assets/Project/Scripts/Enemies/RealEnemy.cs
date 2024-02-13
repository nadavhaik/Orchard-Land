using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyState {Idle, Fighting, Attacking ,WalkingBack}

public class RealEnemy : Enemy
{
    // Start is called before the first frame update
    public float speed = 10f;
    public Camera lineOfSight;
    public float yieldDistance = 5f;
    public float minAttackDistance = 2f;
    public float maxDistanceFromStartPoint = 1f;
    public float minTimeBetweenAttacks = 1f;
    public float maxTimeBetweenAttacks = 10f;
    public float stunTime = 3f;
    public float cooldownForPredictableAttacks = 0.5f;
    public EnemySword enemySword;
    public float parryForce = 2f;
    private bool _stunned = false;


    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private EnemyState _currentState = EnemyState.Idle;
    private Rigidbody _rb;
    private Player _player;
    
    void MarkNotStunned() => _stunned = false;
    void Stun()
    {
        _stunned = true;
        _rb.AddForce(-parryForce * transform.forward);
        Invoke(nameof(MarkNotStunned), stunTime);
    }

    
    
    protected override void Start()
    {
        base.Start();
        _player = FindObjectOfType<Player>();
        _rb = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        
        enemySword.opponentDefendedEvent.AddListener(OnDefended);
        enemySword.opponentParriedEvent.AddListener(OnParried);
        enemySword.opponentParriedEvent.AddListener(_player.HandleParry);
    }

    void OnDefended()
    {
        CancelInvoke(nameof(MarkAttackIsDone));
        MarkAttackIsDone();
    }

    void OnParried()
    {
        Debug.Log("Parried");
        CancelInvoke(nameof(MarkAttackIsDone));
        MarkAttackIsDone();
        Stun();
    }

    protected override void AnimateHit()
    {
        
    }

    private bool SeesPlayer() => PointInCameraView(_player.transform.position, lineOfSight) ||
                                 PointInCameraView(_player.sword.transform.position, lineOfSight) ||
                                 PointInCameraView(_player.shield.transform.position, lineOfSight);

    private void Yield()
    {
        transform.LookAt(_startPosition);
        transform.position += transform.forward * Mathf.Min(Time.deltaTime * speed, Vector3.Distance(transform.position, _startPosition));
        if (Vector3.Distance(transform.position, _startPosition) < maxDistanceFromStartPoint)
        {
            transform.rotation = _startRotation;
            _currentState = EnemyState.Idle;
        }
            
    }

    private void Fight()
    {
        var playerPos = _player.transform.position;
        transform.LookAt(playerPos);
        if (Vector3.Distance(transform.position, playerPos) > minAttackDistance)
            transform.position += Time.deltaTime * speed * transform.forward;
        else
        {
            _currentState = EnemyState.Attacking;
            Invoke(nameof(Attack), Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks));
        }
    }

    void MarkAttackIsDone() => _currentState = EnemyState.Fighting;
    void Attack()
    {
        enemySword.SwingPredictably(EnumFuncs.RandomEnumValue<AttackDirection>(), cooldownForPredictableAttacks);
        Invoke(nameof(MarkAttackIsDone), enemySword.swingDuration + cooldownForPredictableAttacks);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        if(_stunned) return;
        if (_currentState == EnemyState.Attacking)
        {
            transform.LookAt(_player.transform.position);
            return;
        }

        if (_currentState == EnemyState.Fighting &&
            Vector3.Distance(_player.transform.position, transform.position) > yieldDistance)
        {
            _currentState = EnemyState.WalkingBack;
        } else if (_currentState != EnemyState.Attacking && SeesPlayer())
        {
            _currentState = EnemyState.Fighting;
        }

        switch (_currentState)
        {
            case EnemyState.Fighting:
                Fight();
                break;
            case EnemyState.WalkingBack:
                Yield();
                break;
        }
        
    }
}
