using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{
    [SerializeField, Range(0f,100f)]private float _maxSpeed = 15f;
    [SerializeField, Range(0f,100f)]private float _maxAcceleration = 50f;
    [SerializeField, Range(0f,100f)]private float _maxAirAcceleration = 45f;

    private Vector2 _direction;
    private Vector2 _desiredVelocity;
    private Vector2 _velocity;
    private Rigidbody2D _rb;
    
    private Ground _ground;

    private float _maxSpeedChange;
    private float _accerleration;
    private bool _onGround;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
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

        _accerleration = _onGround? _maxAcceleration : _maxAirAcceleration;
        _maxSpeedChange = _accerleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeed);

        _rb.velocity = _velocity;
    }

    private void OnMove(InputValue inputValue)
    {
        _direction.x = inputValue.Get<Vector2>().x;
    }

    private void OnReset()
    {
        SceneManager.LoadScene(0);
    }
}
