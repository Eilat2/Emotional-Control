using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // -------- New Input System --------
    private Vector2 moveInput;
    private bool jumpPressedThisFrame;
    // ---------------------------------

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // תנועה
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // קפיצה
        if (jumpPressedThisFrame && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // מאפסים "ירייה" בסוף פריים
        jumpPressedThisFrame = false;
    }

    // -------- Input System callbacks (Send Messages) --------

    // חייב להיות Action בשם "Move"
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // חייב להיות Action בשם "Jump"
    // חשוב: מקבלים InputValue כדי שזה יעבוד יציב
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            jumpPressedThisFrame = true;
    }

    // -------------------------------------------------------

    bool IsGrounded()
    {
        // הגנה שלא יקרוס אם שכחת לחבר
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}
