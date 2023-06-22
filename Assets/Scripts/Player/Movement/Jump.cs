using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Jump : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 8f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 1;
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.7f;
    [SerializeField, Range(0f, 0.3f)] private float _coyoteTimer = 0.2f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.2f;



    private Vector2 _velocity;
    private Rigidbody2D _rb;
    private Ground _ground;

    private int _jumpPhase;
    private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;
    
    private bool _desiredJump, _onGround, _isJumping;

    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        _defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        _desiredJump |= Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetOnGround();
        _velocity = _rb.velocity;

        if(_onGround && _rb.velocity.y == 0)
        {
            _jumpPhase = 0;
            _coyoteCounter = _coyoteTimer;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if(_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        }
        else if(!_desiredJump && _jumpBufferCounter > 0) 
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if(_jumpBufferCounter > 0) 
        {
            JumpAction();
        }

        if(Input.GetButton("Jump") && _rb.velocity.y > 0)
        {
            _rb.gravityScale = _upwardMovementMultiplier;
        }
        else if(!Input.GetButton("Jump") || _rb.velocity.y < 0)
        {
            _rb.gravityScale = _downwardMovementMultiplier;
        }
        else if(_rb.velocity.y == 0)
        {
            _rb.gravityScale = _defaultGravityScale;
        }

        _rb.velocity = _velocity;
    }

    private void JumpAction()
    {
        if(_coyoteCounter > 0f || (_jumpPhase<_maxAirJumps && _isJumping))
        {
            if (_isJumping) 
            {
                _jumpPhase += 1;
            }
            _jumpBufferCounter = 0;
            _coyoteCounter = 0;
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight * _upwardMovementMultiplier);
            _isJumping = true;

            if(_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            }
            _velocity.y += _jumpSpeed;
        }
    }

    //private void OnJump(InputValue inputValue)
    //{
    //    _desiredJump |= inputValue.isPressed;
    //}

}
