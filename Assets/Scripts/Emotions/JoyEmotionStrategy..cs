using UnityEngine;

public class JoyEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float joyJumpForce = 6f;

    [Header("Glide (2nd press)")]
    [SerializeField] private float normalGravity = 4f;
    [SerializeField] private float glideGravity = 1.2f;
    [SerializeField] private float floatUpImpulse = 8f;

    [Header("Stamina (Joy)")]
    [SerializeField] private float glideCostPerSecond = 15f;
    [SerializeField] private Stamina joyStamina;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator joyAnimator;

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;

    private Vector2 moveInput;

    private bool jumpedFromGround = false;
    private bool glideEnabled = false;

    private bool jumpHeld = false;
    private float holdGraceTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    void Start()
    {
        rb.gravityScale = normalGravity;
        ResolveJoyStamina();
        ResolveJoyAnimator();
    }

    public void Enter()
    {
        rb.gravityScale = normalGravity;

        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", false);
        }
    }

    public void Exit()
    {
        glideEnabled = false;
        rb.gravityScale = normalGravity;

        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", false);
        }
    }

    public void HandleMove(Vector2 move)
    {
        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        jumpHeld = isHeld;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        bool grounded = IsGrounded();

        // נחתנו
        if (grounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpedFromGround = false;
            glideEnabled = false;
            rb.gravityScale = normalGravity;
        }

        // הפסקת ריחוף
        if (glideEnabled && releasedThisFrame)
        {
            glideEnabled = false;
            rb.gravityScale = normalGravity;
            return;
        }

        // קפיצה ראשונה
        if (pressedThisFrame && grounded)
        {
            jumpedFromGround = true;
            glideEnabled = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * joyJumpForce, ForceMode2D.Impulse);
            return;
        }

        // ריחוף
        if (pressedThisFrame && !grounded && jumpedFromGround && !glideEnabled)
        {
            if (joyStamina != null && joyStamina.currentStamina <= 0f)
                return;

            glideEnabled = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);
        }
    }

    public void Tick()
    {
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תנועה
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // אנימציית הליכה
        if (joyAnimator != null)
            joyAnimator.SetFloat("speed", Mathf.Abs(moveInput.x));

        // 🎯 פה הקסם של הריחוף
        if (joyAnimator != null)
            joyAnimator.SetBool("isGliding", glideEnabled);

        if (!glideEnabled)
        {
            rb.gravityScale = normalGravity;
            return;
        }

        // ריחוף פעיל
        if (jumpHeld)
        {
            float cost = glideCostPerSecond * Time.deltaTime;

            if (joyStamina != null && !joyStamina.Use(cost))
            {
                glideEnabled = false;
                rb.gravityScale = normalGravity;
                return;
            }

            rb.gravityScale = glideGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void ResolveJoyStamina()
    {
        if (joyStamina != null) return;

        Stamina[] all = GetComponentsInChildren<Stamina>();

        foreach (var s in all)
        {
            if (s.type == Stamina.StaminaType.Joy)
            {
                joyStamina = s;
                return;
            }
        }
    }

    private void ResolveJoyAnimator()
    {
        if (joyAnimator != null) return;

        Transform joyVisual = transform.Find("JoyVisual");

        if (joyVisual != null)
            joyAnimator = joyVisual.GetComponent<Animator>();
    }
}