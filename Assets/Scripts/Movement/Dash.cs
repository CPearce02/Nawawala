using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private bool canDash = true;
    private bool isDashing ; 
    [SerializeField] private float dashingPower ;
    [SerializeField] private float dashingTime ;
    [SerializeField] private float dashingCooldown;

    private bool dashPressed;

    private TrailRenderer tr;
    private Rigidbody2D rb;
    private Move m;
    private Jump j;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        j = GetComponent<Jump>();
        m = GetComponent<Move>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator DashMovement()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        j.enabled = false;
        rb.velocity = new Vector2(rb.velocity.x *dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        j.enabled = true;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;

    }

    private void OnDash(InputValue inputValue)
    {
        if(canDash)
        {
            StartCoroutine("DashMovement");
        }
    }
}
