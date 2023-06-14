using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Jump : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 8f;
    [SerializeField, Range(0, 5)] private int maxAirJumps = 1;
    [SerializeField, Range(0f, 5f)] private float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 1.7f;

    private Vector2 velocity;
    private Rigidbody2D rb;
    private Ground ground;

    private int jumpPhase;
    private float defaultGravityScale;
    
    private bool desiredJump;
    private bool onGround;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();

        defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();
        velocity = rb.velocity;

        if(onGround)
        {
            jumpPhase = 0;
        }

        if(desiredJump)
        {
            desiredJump = false;
            JumpAction();
        }

        if(rb.velocity.y > 0)
        {
            rb.gravityScale = upwardMovementMultiplier;
        }
        else if(rb.velocity.y < 0)
        {
            rb.gravityScale = downwardMovementMultiplier;
        }
        else if(rb.velocity.y == 0)
        {
            rb.gravityScale = defaultGravityScale;
        }

        rb.velocity = velocity;
    }

    private void JumpAction()
    {
        if(onGround || jumpPhase<maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            if(velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            velocity.y += jumpSpeed;
        }
    }

    private void OnJump(InputValue inputValue)
    {
        desiredJump |= inputValue.isPressed;
    }

}
