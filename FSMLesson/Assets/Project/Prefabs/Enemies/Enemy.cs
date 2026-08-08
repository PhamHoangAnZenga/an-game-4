using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private readonly int IDLE = Animator.StringToHash("EnemyIdle");
    private readonly int ACTIVE = Animator.StringToHash("EnemyActive");

    private enum EnemyState
    {
        Initialize,
        Idle,
        Danger
    }

    private enum Attack
    {
        Delay,
        Move
    }

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;

    [Header("Config")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _idleTime = 2f;
    [SerializeField] private float _delayTime = 0.5f;

    [Header("Stand Position")]
    [SerializeField] private Transform[] _standPos;

    private EnemyState _state;
    private Attack _attack;

    private float _timer;

    private int _currentPos;

    private bool _gameOn = true;

    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {
        _state = EnemyState.Initialize;

        while (_gameOn)
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
            }

            yield return 0;
        }
    }

    private void InitState()
    {
        _state = EnemyState.Idle;
        _timer = Time.time + _idleTime;

        _currentPos = Random.Range(0, 2);
        transform.position = _standPos[_currentPos].position;
    }

    private void IdleState()
    {
        if (_timer < Time.time)
        {
            _state = EnemyState.Danger;

            _attack = Attack.Delay;
            _timer = Time.time + _delayTime;

            _animator.Play(ACTIVE);
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

            Vector3 target = _standPos[_currentPos].position;
            target.y = transform.position.y;
            _rb.linearVelocity = (target - transform.position).normalized * _moveSpeed;

        }
    }

    private void Move()
    {
        Vector3 target = _standPos[_currentPos].position;
        target.y = transform.position.y;

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _state = EnemyState.Idle;

            _animator.Play(IDLE);

            _timer = Time.time + _idleTime;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.GameOver();
            _gameManager.GameOver();
            _gameOn = false;     
        }
    }
}
