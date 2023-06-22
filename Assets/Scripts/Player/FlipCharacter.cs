using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCharacter : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float flipThreshold = 0.1f;

    private bool _isFacingRight = true;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        float xVelocity = rb.velocity.x;

        if (Mathf.Abs(xVelocity) > flipThreshold)
        {
            bool shouldFlip = xVelocity > 0;
            Flip(shouldFlip);
        }
    }

    private void Flip(bool shouldFlip)
    {
        if (shouldFlip != _isFacingRight)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;

            _isFacingRight = shouldFlip;
        }
    }
}
