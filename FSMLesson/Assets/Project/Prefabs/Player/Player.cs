using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

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
    [SerializeField] private Camera _camera;

    [Header("Config")]
    [SerializeField] private float _idleTime = 1.5f;
    [SerializeField] private float _activeTime = 0.5f;


    private PlayerState _state;
    private float _timer;

    private bool _moveFlag = false;
    private bool _controlFlag = false;

    private LayerMask _playerMask;
    private LayerMask _groundMask;

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
    
    void Update()
    {
        if (_controlFlag)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray, _playerMask))
            {
                _moveFlag = true;                
            }
            else
            {
                _moveFlag = false;
            }
        }
        else
        {
            _moveFlag = false;
        }
    }

    void FixedUpdate()
    {
        if (_moveFlag)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray, out RaycastHit hit, _groundMask))
            {
                _rb.MovePosition(hit.point);
            }
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

            _controlFlag = true;

            _timer = Time.time + _activeTime;
        }
    }

    private void ActiveState()
    {
        if (_timer < Time.time)
        {
            _state = PlayerState.Idle;

            _animator.Play(_IDLE);

            _controlFlag = false;

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
