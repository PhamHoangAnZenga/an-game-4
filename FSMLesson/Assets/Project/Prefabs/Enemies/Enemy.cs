using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private readonly int _IDLE = Animator.StringToHash("EnemyIdle");
    private readonly int _ACTIVE = Animator.StringToHash("EnemyActive");

    private enum EnemyState
    {
        Initialize,
        Idle,
        Danger,
        EndGame,
    }

    private enum Attack
    {
        Delay,
        Move
    }


    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;

    [Header("Config")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _idleTime = 2f;
    [SerializeField] private float _delayTime = 0.5f;


    [Header("Stand Position")]
    [SerializeField] private Transform[] _standPos;

    private EnemyState _state;
    private Attack _attack;

    private float _timer;

    private bool _moveFlag = false;

    private int _currentPos;

    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {
        _state = EnemyState.Initialize;

        while (true)
        {
            switch (_state)
            {                
                case EnemyState.Initialize:
                    InitState();
                    break;
                case EnemyState.Idle:
                    IdleState();
                    break;
                case EnemyState.Danger:
                    DangerState();
                    break;
                case EnemyState.EndGame:
                    yield break;
            }

            yield return 0;
        }
    }

    void FixedUpdate()
    {
        if (_moveFlag)
        {
            Vector3 target = _standPos[_currentPos].position;
            target.y = transform.position.y;

            Vector3 newPos = Vector3.MoveTowards(
                transform.position,
                target,
                _moveSpeed * Time.fixedDeltaTime
            );

            _rb.MovePosition(newPos);
        }
    }

    private void InitState()
    {
        _state = EnemyState.Idle;
        _timer = Time.time + _idleTime;

        _currentPos = Random.Range(0, 2);
        transform.position = _standPos[_currentPos].position;
        Debug.Log(transform.position);
    }

    private void IdleState()
    {
        if (_timer < Time.time)
        {
            _state = EnemyState.Danger;

            _attack = Attack.Delay;
            _timer = Time.time + _delayTime;


            _animator.Play(_ACTIVE);
        }
    }

    private void DangerState()
    {
        switch (_attack)
        {
            case Attack.Delay:
                Delay();
                break;
            case Attack.Move:
                Move();
                break;        
        }
    }

    private void Delay()
    {
        if(_timer < Time.time)
        {
            _attack = Attack.Move;

            _currentPos = 1 - _currentPos;
            _moveFlag = true;
        }
    }
    
    private void Move()
    {
        Vector3 target = _standPos[_currentPos].position;
        target.y = transform.position.y;

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _state = EnemyState.Idle;

            _animator.Play(_IDLE);

            _moveFlag = false;
            _timer = Time.time + _idleTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            _state = EnemyState.EndGame;
        }
    }
}
