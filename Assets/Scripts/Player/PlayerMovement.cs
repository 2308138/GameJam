using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 12F;
    public float jumpForce = 16F;
    public float coyoteTime = 0.15F;
    private float coyoteCounter = 0F;

    [Header("Dash Settings")]
    public float dashSpeed = 25F;
    public float dashDuration = 0.2F;
    public float dashCooldown = 1.2F;
    private bool isDashing = false;
    private bool canDash = true;

    // Private Variables
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        // Simple Ground Check
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1F, LayerMask.GetMask("Ground"));

        // Coyote Time Logic
        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Space") && coyoteCounter > 0F)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTime = 0F;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0F;

        // Dash Logic
        float dashDir = moveInput != 0 ? moveInput : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0F);

        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}