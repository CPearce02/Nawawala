using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]private Animator _playerAnim;
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private SpriteRenderer _spriteRenderer;
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
    private bool _isDashing;
    private Vector2 _dashingDir;
    //[SerializeField]private float _dashCoolDown;

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
        _canDash = false;
    }

    private void AddExtraJump(object sender, EventArgs e)
    {
        _maxNumberOfExtraJumps++;
    }

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
            if(rb.velocity.y < 0 && !Grounded)
            {
                rb.gravityScale = _gravityScale + _fallGravityModifier;
                //_extraMoveSpeed = 1.5f;
            }
            else
            {
                rb.gravityScale = _gravityScale;
                //_extraMoveSpeed = 1f;
            }
        }
        else
        {
            rb.gravityScale = 0;
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
        // if(moveInput != 0)
        // {
        //     if(moveInput < 0)
        //     {
        //         _spriteRenderer.flipX = true;
        //         //transform.rotation = Quaternion.Euler(0,180f,0);
        //     }
        //     else
        //     {
        //         if(moveInput > 0)
        //         {
        //             _spriteRenderer.flipX = false;
        //             //transform.rotation = Quaternion.Euler(0,0,0);
        //         }
        //     }
        // }
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
        if (_canDash)
        {
            Dash();
        }
    }

    private void Dash()
    {
        if(_moveInput > 0)
        {
            _dashingDir = new Vector2(1,0);
        }
        else if(_moveInput < 0)
        {
            _dashingDir = new Vector2(-1,0);
        }
        else
        {
            return;
        }

        _canDash = false;
        //_extraMoveSpeed = 1.7f;
        _isDashing = true;
        //_dashCoolDown = 0.15f;

        StartCoroutine(StopDashing());
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(_dashingTime);
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
        //_isJumping = false; 
    }

    private void Dashing()
    {
        if(_isDashing)
        {
            rb.velocity = _dashingDir * _dashingVelocity;
        }
    }

    private void CheckingGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_col2D.bounds.center, _col2D.bounds.size, 0f, Vector2.down, 1, _platformLayer);
        if(raycastHit.collider != null)
        {
            _canDash = true;
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