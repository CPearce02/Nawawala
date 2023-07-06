using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField, Range(0f, 100f)] private float _maxSpeed = 15f;
    [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 50f;
    [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 45f;

    private Vector2 _direction;
    private Vector2 _desiredVelocity;
    private float _maxSpeedChange;
    private float _accerleration;
    private bool _moveLocked;

    [Header("Jump")]
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 8f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 1;
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.7f;
    [SerializeField, Range(0f, 0.3f)] private float _coyoteTimer = 0.2f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.2f;

    private int _jumpPhase;
    private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;
    private bool _desiredJump, _onGround, _isJumping, _jumpLocked;

    [Header("Dash")]
    [SerializeField] private float _dashingPower;
    [SerializeField] private float _dashingTime;
    [SerializeField] private float _dashingCooldown;

    private bool _canDash = true;
    private bool _isDashing, _dashLocked;

    private Vector2 _velocity;
    private Rigidbody2D _rb;
    private Ground _ground;
    private PlayerInput _inputActions;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        _inputActions = GetComponent<PlayerInput>();
        _defaultGravityScale = 1f;

        //Base
        SetAirJump(0);
        GameEvents.onAbilityLock("Dash", true);
    }

    private void OnEnable()
    {
        GameEvents.onAbilityLock += LockAbility;
    }

    private void OnDisable()
    {
        GameEvents.onAbilityLock -= LockAbility;
    }

    // Update is called once per frame
    void Update()
    {
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _ground.GetFriction(), 0f);
    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetOnGround();
        _velocity = _rb.velocity;

        if (_onGround && _rb.velocity.y == 0)
        {
            _jumpPhase = 0;
            _coyoteCounter = _coyoteTimer;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        }
        else if (!_desiredJump && _jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0)
        {
            JumpAction();
        }

        if (_inputActions.actions["Jump"].inProgress && _rb.velocity.y > 0)
        {
            _rb.gravityScale = _upwardMovementMultiplier;
        }
        else if (!_inputActions.actions["Jump"].inProgress || _rb.velocity.y < 0)
        {
            _rb.gravityScale = _downwardMovementMultiplier;
        }
        else if (_rb.velocity.y == 0)
        {
            _rb.gravityScale = _defaultGravityScale;
        }

        _accerleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
        _maxSpeedChange = _accerleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeed);

        _rb.velocity = _velocity;
    }

    private void JumpAction()
    {
        if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJumps && _isJumping))
        {
            if (_isJumping)
            {
                _jumpPhase += 1;
            }
            _jumpBufferCounter = 0;
            _coyoteCounter = 0;
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight * _upwardMovementMultiplier);
            _isJumping = true;

            if (_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            }

            _velocity.y += _jumpSpeed;
        }
    }

    private void OnJump(InputValue inputValue)
    {
        _desiredJump |= inputValue.isPressed;
    }

    private IEnumerator DashMovement()
    {
        _canDash = false;
        _isDashing = true;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        _rb.velocity = new Vector2(_rb.velocity.x * _dashingPower, 0);
        yield return new WaitForSeconds(_dashingTime);
        _rb.gravityScale = originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
    }

    private void OnDash(InputValue inputValue)
    {
        if (_canDash)
        {
            StartCoroutine("DashMovement");
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _direction.x = inputValue.Get<Vector2>().x;
    }

    private void LockAbility(string ability, bool lockState) 
    {
        if (lockState) 
        {
            if(ability == "Jump") 
            {
                SetAirJump(0);
            }
            else 
            {
                _inputActions.actions[ability].Disable();
            }
        }
        else 
        {
            if(ability == "Jump") 
            {
                SetAirJump(1);
            }
            else 
            {
                _inputActions.actions[ability].Enable();
            }
        }
    }

    private void SetAirJump(int amount) 
    {
        _maxAirJumps = amount;
    }

}
