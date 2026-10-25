
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isFacingRight = true;

   

    // Dash variables
    private bool canDash = true;
    private bool isDashing = false;
    private float dashDuration = 0.2f;
    private float dashPower = 24f;
    private float dashCooldown = 1f;

    


    void Update()
    {
        

        if (isDashing)
        {
            return;
        }

            // Flip the player sprite based on movement direction
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        
    }


        // Jumping logic
    public void Jump(InputAction.CallbackContext context)
{
    if (context.performed)
    {
        Debug.Log("Performed fired. Grounded = " + IsGrounded());
    }

    if (context.performed && IsGrounded())
    {
        Debug.Log("Jumping");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
    }

    if (context.canceled && rb.linearVelocity.y > 0f)
    {
        Debug.Log("Jump Canceled");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
    }
}// Jumpoing logic

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

        // Dash logic
    /*public void Dash(InputAction.CallbackContext context)
    {
        
        float dashMultiplier = 2.3f; // Adjust this value to control the dash distance
        
        

        if (context.performed && canDash == true)
        {

            Debug.Log("Dashing");
            rb.MovePosition(rb.position + new Vector2(rb.linearVelocity.x, 0f) * speed * Time.fixedDeltaTime * dashMultiplier);
            canDash = false; // Disable dashing after the first dash
            
        }

        
    }*/


    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashPower, 0f);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    //AI help

    private void OnDrawGizmosSelected()
{
    if (groundCheck == null) return;

    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(
        groundCheck.position,
        groundCheckRadius
    );
}

    
}