using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // -------- New Input System --------
    private Vector2 moveInput;
    private bool jumpPressed;
    // ---------------------------------

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
        jumpPressed = false; // מאפסים בסוף הפריים
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (jumpPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // -------- Input System callbacks --------

    // Action בשם "Move"
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Action בשם "Jump"
    public void OnJump()
    {
        jumpPressed = true;
    }

    // ----------------------------------------

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}
