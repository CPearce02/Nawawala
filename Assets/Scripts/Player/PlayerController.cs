using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private Transform _spriteRenderer;
    [SerializeField]private Collider2D _col2D;
    private PlayerInput _inputActions;


    [Header("Movement Variables")]
    [SerializeField, Range(0f, 50f)]private float _movementSpeed;
    [SerializeField, Range(1, 10f), Tooltip("Has To be the Value of 1")]private float _extraMoveSpeed;
    [SerializeField, Range(0f, 30f)]private float _acceleration;
    [SerializeField, Range(0f, 30f)]private float _deceleration;
    [SerializeField, Range(0f, 1f)]private float _velPower;
    [SerializeField, Range(0f, 1f)]private float _frictionAmount;
    private float _moveInput;
    

    [Header("Jumping Variables")]
    [SerializeField, Range(0f, 50f)]private float _jumpForce;
    //[SerializeField]private bool _isJumping;
    [SerializeField, Range(0, 5)]private int _maxNumberOfExtraJumps;
    private int _numberOfExtraJumpsLeft;
    [SerializeField]private float _jumpCutModifier;
    [SerializeField]private float _jumpBufferTotalTime;
    private bool _isJumpBuffer;
    private float _jumpBufferTimer;
    [SerializeField]private float _coyoteTotalTime;
    private float _coyoteTimer;
    // [SerializeField]private bool jumpInputReleased;
    [SerializeField]private LayerMask _platformLayer;
    [SerializeField]private float _apexModifier;
    private bool _justOffTheGround;
    private bool _jumpInputValue;
    [SerializeField]bool _grounded = false;
    public bool Grounded { get => _grounded; private set => _grounded = value; }


    [Header("Gravity")]
    [SerializeField]private float _gravityScale;
    [SerializeField]private float _fallGravityModifier;
    [SerializeField]private float _clampFallSpeed;


    [Header("Dash Variables")]
    [SerializeField]private float _dashingVelocity = 5f;
    [SerializeField]private float _dashingTime = 0.5f; 
    [SerializeField]private bool _canDash = true;
    [SerializeField]private float _dashTotalCoolDownTime;
    [Tooltip("No Need to touch")]
    [SerializeField]private float _dashCoolDownTimer;
    private bool _canRestoreDash;
    private bool _dashIsOnCoolDown;
    private bool _isDashing;
    private bool _hasTouchedGroundWhileOnCooldDown;
    private Vector2 _dashingDir;
    public bool _dashUnlocked{get; set;}

    
    [Header("Effects")]
    [SerializeField] private Transform _jumpEffectSpot;
    [SerializeField] private GameObject _jumpEffectObj;

    [Header("Animations")]
    [SerializeField] private Animator _playerAnim;
    private string _currentAnimPlaying;
    const string RUN = "Run";
    const string IDLE = "Idle";
    
    private void Start() 
    {
        _inputActions = GetComponent<PlayerInput>();
    }

    public void Init(PlayerManager playerManager)
    {
        playerManager._addExtraPlayerJump += AddExtraJump;
    }

    private void OnEnable() 
    {
        transform.rotation = Quaternion.Euler(0,0,0);
        //_canDash = false;
    }

    private void AddExtraJump(object sender, EventArgs e)
    {
        _maxNumberOfExtraJumps++;
    }

    bool _isPlayerFacingRight;

    void Update()
    {
        CheckingGround();

        //Allow player to press a bit before and still jump when actually landing on the ground
        if(_isJumpBuffer)
        {
            if(_jumpBufferTimer < 0)
            {
                _isJumpBuffer = false;
            }
            if(Grounded)
            {
                Jump();
                _numberOfExtraJumpsLeft = _maxNumberOfExtraJumps;
            }
            _jumpBufferTimer -= Time.deltaTime;
        }

        //Gravity changes
        if(!_isDashing){
            if(rb.velocity.y < _apexModifier && !Grounded)
            {
                rb.gravityScale = _gravityScale + _fallGravityModifier;
            }
            else
            {
                rb.gravityScale = _gravityScale;
            }
        }
        else
        {
            rb.gravityScale = 0;
        }

        //Dash CoolDown
        if(_dashIsOnCoolDown)
        {
            if(_dashCoolDownTimer < 0)
            {
                if(_hasTouchedGroundWhileOnCooldDown)
                {
                    _canDash = true;
                    _hasTouchedGroundWhileOnCooldDown = false;
                }
                else
                {
                    _canRestoreDash = true;
                }
                _dashIsOnCoolDown = false;
            }   
            else
            {
                _dashCoolDownTimer -= Time.deltaTime;
            }
        }

        //Unsure
        // if(rb.velocity.y > 40)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, 40);
        // }

        //Control Fall Speed
        if(rb.velocity.y < -_clampFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -_clampFallSpeed);
        }

        //Change Direction of Sprite
        if(rb.velocity.x != 0)
        {
            if(rb.velocity.x < -0.05)
            {
                _isPlayerFacingRight = false;
                _spriteRenderer.rotation = Quaternion.Euler(0,180f,0);
            }
            else if(rb.velocity.x > 0.05)
            {
                //_spriteRenderer.flipX = false;
                _isPlayerFacingRight = true;
                _spriteRenderer.rotation = Quaternion.Euler(0,0,0);
            }
            SoundManager.Instance.PlayRepeatingSound(SoundManager.GameSoundType.PlayerStep, this, 0.5f);
        }

        //if(_moveInput != 0)
        //{
        //    if(_currentAnimPlaying != RUN)
        //    {
        //        _playerAnim.Play(RUN);
        //    }
        //}
        //else
        //{
        //    if(_currentAnimPlaying != IDLE)
        //    {
        //        _playerAnim.Play(IDLE);
        //    }
        //}

        _playerAnim.SetFloat("Speed", MathF.Abs(rb.velocity.x));
    }

    void FixedUpdate()
    {
        Move();
        Dashing();
        Friction();
    }   

    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>().x * _movementSpeed;
    }
    
    private void OnJump(InputValue inputValue)
    {
        _jumpInputValue = inputValue.isPressed;

        if(_jumpInputValue)
        {
            if(Grounded)
            {
                Jump();
                _numberOfExtraJumpsLeft = _maxNumberOfExtraJumps;
            }
            else if(_numberOfExtraJumpsLeft > 0)
            {
                _numberOfExtraJumpsLeft--;
                Jump();
            }
            else
            {
                JumpBuffer();
            }
        }
        else
        {
            //Jump button was let go
            JumpQuickRelease();
        }
    }

    //Unsure whats better?
    // IEnumerator JumpBuffer()
    // {
    //     yield return new WaitForSeconds(_jumpBufferTime);
    // }
    
    private void JumpBuffer()
    {
        _isJumpBuffer = true;
        _jumpBufferTimer = _jumpBufferTotalTime;
    }

    private void JumpQuickRelease()
    {
        if(rb.velocity.y > 0)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - _jumpCutModifier), ForceMode2D.Impulse);
        }
    }

    private void OnDash(InputValue inputValue)
    {
        if(_dashUnlocked)
        {
            if (_canDash)
            {
                Dash();
            }
        }
    }

    private void Dash()
    {
        // if(_moveInput > 0)
        // {
        //     _dashingDir = new Vector2(1,0);
        // }
        // else if(_moveInput < 0)
        // {
        //     _dashingDir = new Vector2(-1,0);
        // }
        // else
        // {
        //     return;
        // }
        
        if(_isPlayerFacingRight)
        {
            _dashingDir = new Vector2(1,0);
        }
        else if(!_isPlayerFacingRight)
        {
            _dashingDir = new Vector2(-1,0);
        }

        _canDash = false;
        _isDashing = true;

        //StartCoroutine(StopDashing());
        Invoke(nameof(StopDashing), _dashingTime);
    }

    private void StopDashing()
    {
        _dashCoolDownTimer = _dashTotalCoolDownTime;
        _dashIsOnCoolDown = true;
        _isDashing = false;
    }
    
    private void Move()
    {
        //rb.velocity = new Vector2(movement, rb.velocity.y);
        float speedDif = _moveInput - rb.velocity.x;

        float accelRate = (Mathf.Abs(_moveInput) > 0.01f) ? _acceleration : _deceleration;

        float move = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, _velPower) * Mathf.Sign(speedDif);

        rb.AddForce(move * _extraMoveSpeed * Vector2.right);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        //Going up slopes the player will be way faster
        // if(rb.velocity.y < 0)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, 0);
        // }

        rb.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse); 
        Instantiate(_jumpEffectObj, _jumpEffectSpot.position, Quaternion.identity, _jumpEffectSpot);
        SoundManager.Instance.PlaySound2D(SoundManager.GameSoundType.PlayerJump);
        //_isJumping = false; 
    }

    private void Dashing()
    {
        if(_isDashing)
        {
            rb.velocity = _dashingDir * _dashingVelocity;
        }
    }

    float _extraHeight = 0.05f;
    private void CheckingGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_col2D.bounds.center, new Vector2(_col2D.bounds.size.x, _col2D.bounds.size.y), 0f, Vector2.down, _extraHeight, _platformLayer);
        if(raycastHit.collider != null)
        {
            if(_dashIsOnCoolDown)
            {
                _hasTouchedGroundWhileOnCooldDown = true;
            }
            else if(_canRestoreDash)
            {
                _canRestoreDash = false;
                _canDash = true;
            }

            Grounded = true;
            _justOffTheGround = true;
        }
        else if(Grounded)
        {
            if(_justOffTheGround)
            {
                _coyoteTimer = _coyoteTotalTime;
                _justOffTheGround = false;
            }

            if(_coyoteTimer < 0)
            {
                Grounded = false;
            }
            _coyoteTimer -= Time.deltaTime;
        }

        

        #if UNITY_EDITOR
        Color rayColor;
        if(raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(_col2D.bounds.center + new Vector3(_col2D.bounds.extents.x, 0), Vector2.down * (_col2D.bounds.extents.y + _extraHeight), rayColor);
        Debug.DrawRay(_col2D.bounds.center - new Vector3(_col2D.bounds.extents.x, 0), Vector2.down * (_col2D.bounds.extents.y + _extraHeight), rayColor);
        Debug.DrawRay(_col2D.bounds.center - new Vector3(_col2D.bounds.extents.x, _col2D.bounds.extents.y + _extraHeight), Vector2.right * (_col2D.bounds.extents.x), rayColor);
        #endif
    }

    private void Friction()
    {
        if(Mathf.Abs(_moveInput) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(_frictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
}
