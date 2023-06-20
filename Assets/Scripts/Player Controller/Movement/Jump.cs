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

    private Vector2 _velocity;
    private Rigidbody2D _rb;
    private Ground _ground;

    private int _jumpPhase;
    private float _defaultGravityScale;
    
    private bool _desiredJump;
    private bool _onGround;

    
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

    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetOnGround();
        _velocity = _rb.velocity;

        if(_onGround)
        {
            _jumpPhase = 0;
        }

        if(_desiredJump)
        {
            _desiredJump = false;
            JumpAction();
        }

        if(_rb.velocity.y > 0)
        {
            _rb.gravityScale = _upwardMovementMultiplier;
        }
        else if(_rb.velocity.y < 0)
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
        if(_onGround || _jumpPhase<_maxAirJumps)
        {
            _jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
            if(_velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }
            _velocity.y += jumpSpeed;
        }
    }

    private void OnJump(InputValue inputValue)
    {
        _desiredJump |= inputValue.isPressed;
    }

}
