using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0F;
    public float jumpForce = 0F;
    private bool movementEnabled = true;

    [Header("Dash Settings")]
    public float dashSpeed = 0F;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Better Physics Settings")]
    public float fallMultiplier = 0F;
    public float lowJumpMultiplier = 0F;

    [Header("Advanced Jump Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public int maxJumps = 0;
    private int jumpsRemaining = 0;
    public float jumpBufferTime = 0F;
    private float jumpBufferCounter = 0F;
    public float coyoteTime = 0F;
    private float coyoteCounter = 0F;

    [Header("Wall Mechanics Settings")]
    public Transform wallCheck;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 0F;
    private bool isWallClinging = false;

    // Private References
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private PlayerController controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerController();

        controls.Player.Dash.performed += _ => StartCoroutine(PerformDash());
        controls.Player.Jump.performed += _ => Jump();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    private void Update()
    {
        if (isDashing) return;
        if (!movementEnabled) return; 

        moveInput = controls.Player.Move.ReadValue<Vector2>();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2F, groundLayer);
        bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2F, wallLayer);

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpsRemaining = maxJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (controls.Player.Jump.triggered)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isTouchingWall && !isGrounded && moveInput.x != 0F)
        {
            isWallClinging = true;
        }
        else
        {
            isWallClinging = false;
        }

        if (jumpBufferCounter > 0F)
        {
            if (coyoteCounter > 0F || jumpsRemaining > 0 || isWallClinging)
            {
                Jump();
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
        if (!movementEnabled) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0F)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0F && !controls.Player.Jump.IsPressed())
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (coyoteCounter <= 0F && !isWallClinging)
        {
            jumpsRemaining--;
        }

        jumpBufferCounter = 0;
        coyoteCounter = 0;
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0F;

        // Dash Logic
        float dashDir = moveInput.x != 0 ? moveInput.x : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0F);

        yield return new WaitForSeconds(0.2F);
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(1.2F);
        canDash = true;
    }

    public void ToggleMovement(bool state)
    {
        movementEnabled = state;
        if (!state)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}