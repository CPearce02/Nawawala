using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField, Range(0f,100f)]private float maxSpeed = 15f;
    [SerializeField, Range(0f,100f)]private float maxAcceleration = 50f;
    [SerializeField, Range(0f,100f)]private float maxAirAcceleration = 45f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D rb;
    
    private Ground ground;

    private float maxSpeedChange;
    private float accerleration;
    private bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
    }

    // Update is called once per frame
    void Update()
    {
        // direction.x = input.RetrieveMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(maxSpeed - ground.GetFriction(), 0f);
    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();
        velocity = rb.velocity;

        accerleration = onGround? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = accerleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeed);

        rb.velocity = velocity;
    }

    private void OnMove(InputValue inputValue)
    {
        direction.x = inputValue.Get<Vector2>().x;
    }
}
