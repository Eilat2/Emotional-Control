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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;

        ResolveJoyStamina();
        ResolveJoyAnimator();
    }

    public void Enter()
    {
        rb.gravityScale = normalGravity;

        glideEnabled = false;
        jumpHeld = false;

        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", false);
        }
    }

    public void Exit()
    {
        glideEnabled = false;
        jumpHeld = false;

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

        // אם נחתנו על הרצפה — מאפסים קפיצה/ריחוף
        if (grounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpedFromGround = false;
            glideEnabled = false;
            rb.gravityScale = normalGravity;
        }

        // אם משחררים Space בזמן ריחוף — מפסיקים ריחוף
        if (glideEnabled && releasedThisFrame)
        {
            glideEnabled = false;
            rb.gravityScale = normalGravity;
            return;
        }

        // לחיצה ראשונה מהקרקע = קפיצה
        if (pressedThisFrame && grounded)
        {
            jumpedFromGround = true;
            glideEnabled = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * joyJumpForce, ForceMode2D.Impulse);

            return;
        }

        // לחיצה שנייה באוויר = הפעלת ריחוף
        if (pressedThisFrame && !grounded && jumpedFromGround && !glideEnabled)
        {
            ResolveJoyStamina();

            // אם אין סטאמינה — לא נכנסים לריחוף
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
        {
            if (joyAnimator != null)
            {
                joyAnimator.SetFloat("speed", 0f);
                joyAnimator.SetBool("isGliding", false);
            }

            return;
        }

        // תנועה אופקית
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // האם השחקנית באמת מרחפת כרגע בפועל
        bool isActuallyGliding = glideEnabled && jumpHeld;

        // עדכון אנימציות
        if (joyAnimator != null)
        {
            // אם מרחפים — לא מפעילים Walk באוויר
            joyAnimator.SetFloat("speed", isActuallyGliding ? 0f : Mathf.Abs(moveInput.x));

            // מפעיל/מכבה את אנימציית הריחוף
            joyAnimator.SetBool("isGliding", isActuallyGliding);
        }

        // אם הריחוף לא מופעל בכלל — כבידה רגילה
        if (!glideEnabled)
        {
            rb.gravityScale = normalGravity;
            return;
        }

        // אם הריחוף מופעל אבל לא מחזיקים Space — כבידה רגילה
        if (!jumpHeld)
        {
            rb.gravityScale = normalGravity;
            return;
        }

        // אם מחזיקים Space בזמן ריחוף — מורידים סטאמינה ומקטינים כבידה
        ResolveJoyStamina();

        if (joyStamina != null)
        {
            float cost = glideCostPerSecond * Time.deltaTime;

            if (!joyStamina.Use(cost))
            {
                glideEnabled = false;
                rb.gravityScale = normalGravity;

                if (joyAnimator != null)
                    joyAnimator.SetBool("isGliding", false);

                return;
            }
        }

        rb.gravityScale = glideGravity;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("[JOY] groundCheck לא הוגדר באינספקטור!");
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void ResolveJoyStamina()
    {
        if (joyStamina != null) return;

        Stamina[] all = GetComponentsInChildren<Stamina>(true);

        foreach (Stamina s in all)
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