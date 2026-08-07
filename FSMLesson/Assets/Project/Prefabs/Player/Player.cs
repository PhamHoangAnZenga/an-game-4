using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly int _IDLE = Animator.StringToHash("PlayerIdle");
    private readonly int _ACTIVE = Animator.StringToHash("PlayerActive");

    private enum PlayerState
    {
        Initialize,
        Idle,
        Active,
        EndGame,
    }


    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;

    [Header("Config")]
    [SerializeField] private float _idleTime = 1.5f;
    [SerializeField] private float _activeTime = 0.5f;


    private PlayerState _state;
    private float _timer;

    private bool _moveFlag = false;


    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {
        _state = PlayerState.Initialize;

        while (true)
        {
            switch (_state)
            {                
                case PlayerState.Initialize:
                    InitState();
                    break;
                case PlayerState.Idle:
                    IdleState();
                    break;
                case PlayerState.Active:
                    ActiveState();
                    break;
                case PlayerState.EndGame:
                    yield break;
            }

            yield return 0;
        }
    }

    void Udpate()
    {
    }

    void FixedUpdate()
    {
        if (_moveFlag)
        {
            
        }
    }

    private void InitState()
    {
        _state = PlayerState.Idle;
        _timer = Time.time + _idleTime;
    }

    private void IdleState()
    {
        if (_timer < Time.time)
        {
            _state = PlayerState.Active;

            _animator.Play(_ACTIVE);

            _moveFlag = true;

            _timer = Time.time + _idleTime;
        }
    }

    private void ActiveState()
    {
        if (_timer < Time.time)
        {
            _state = PlayerState.Idle;

            _animator.Play(_IDLE);

            _moveFlag = false;

            _timer = Time.time + _idleTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            _state = PlayerState.EndGame;
        }
    }
}
