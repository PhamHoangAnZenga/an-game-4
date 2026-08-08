using System.Collections;
using UnityEditor.Callbacks;
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
        GameOver,
    }


    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameManager _gameManager;

    [Header("Config")]
    [SerializeField] private float _idleTime = 1.5f;
    [SerializeField] private float _activeTime = 0.5f;


    private PlayerState _state;
    private float _timer;

    private bool _moveFlag = false;
    private bool _controlFlag = false;

    private LayerMask _playerMask;
    private LayerMask _groundMask;

    private Vector3 _target;

    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();

        _playerMask = LayerMask.GetMask("Player");
        _groundMask = LayerMask.GetMask("Ground");
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
                case PlayerState.GameOver:
                    yield break;
            }

            yield return 0;
        }
    }
    
    void Update()
    {
        if (_controlFlag)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, Mathf.Infinity,  _playerMask))
                {
                    Debug.Log("in");
                    _moveFlag = true;
                }
                else
                {
                    _moveFlag = false;
                }
            }
        }
        else
        {
            _moveFlag = false;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _moveFlag = false;
        }

    }

    void FixedUpdate()
    {
        if (_moveFlag)
        {            
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundMask))
            {
                _target = hit.point;
                _target.y = transform.position.y;   
                _rb.MovePosition(_target);                
            }
        }
    }
    public void GameOver()
    {
        _controlFlag = false;
        _state = PlayerState.GameOver;
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
    private void OnTriggerEnter(Collider colider)
    {
        if (colider.gameObject.TryGetComponent(out Goal goal))
        {
            _state = PlayerState.GameOver;
            GameOver();
            _gameManager.Win();         
        }
    }
}
