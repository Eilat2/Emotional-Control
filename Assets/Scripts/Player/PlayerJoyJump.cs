using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class PlayerJoyJump : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;

    [SerializeField] float normalJumpForce = 3f;  // קפיצה רגילה
    [SerializeField] float joyJumpForce = 6f;     // קפיצה עם שמחה

    [Header("Joy Mode")]
    [SerializeField] bool joyActive = true;

    // אם true: לחיצה אחת מדליקה/מכבה שמחה (Toggle)
    // אם false: שמחה פעילה רק בזמן שמחזיקים את הכפתור (Hold)
    [SerializeField] bool joyToggleMode = true;

    [Header("Glide (Joy)")]
    [SerializeField] float normalGravity = 4f;
    [SerializeField] float glideGravity = 1.2f;

    [Header("Stamina (Joy)")]
    [SerializeField] float glideCostPerSecond = 15f; // כמה סטאמינה יורדת בשנייה בזמן ריחוף

    [Header("Float Up (2nd press)")]
    [SerializeField] float floatUpImpulse = 8f; // דחיפה למעלה בלחיצה השנייה

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.15f;
    [SerializeField] LayerMask groundLayer;

    private Rigidbody2D rb;

    // סטאמינה של שמחה בלבד (Joy)
    private Stamina joyStamina;

    private bool jumpedFromGround = false; // האם כבר קפצנו מהרצפה (לחיצה ראשונה)
    private bool glideEnabled = false;     // האם ריחוף פעיל כרגע

    // ----------- New Input System state -----------
    private Vector2 moveInput;
    private bool jumpHeld = false;

    // אירועים רגעיים: true רק בפריים שבו הכפתור נלחץ/שוחרר
    private bool jumpPressedThisFrame = false;
    private bool jumpReleasedThisFrame = false;

    // Joy hold state (בשביל מצב "החזקה")
    private bool joyHeld = false;
    // ---------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;

        joyStamina = GetStamina(Stamina.StaminaType.Joy);
    }

    void Update()
    {
        // ---------- תנועה ----------
        float move = moveInput.x;
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        bool grounded = IsGrounded();

        // ---------- Joy Mode (Toggle / Hold) ----------
        if (!joyToggleMode)
            joyActive = joyHeld;

        // אם שחררנו את כפתור הקפיצה בזמן ריחוף — מפסיקים מיד
        if (glideEnabled && jumpReleasedThisFrame)
        {
            glideEnabled = false;
            rb.gravityScale = normalGravity;
        }

        // איפוס רק כשבאמת "נחתנו"
        if (grounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpedFromGround = false;
            glideEnabled = false;
            rb.gravityScale = normalGravity;
        }

        // ---------- לחיצה ראשונה: קפיצה (רק על הקרקע) ----------
        if (jumpPressedThisFrame && grounded)
        {
            float jumpForce = joyActive ? joyJumpForce : normalJumpForce;

            jumpedFromGround = true;
            glideEnabled = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // ---------- לחיצה שנייה: ריחוף + דחיפה למעלה ----------
        if (jumpPressedThisFrame && !grounded && jumpedFromGround && !glideEnabled)
        {
            if (joyStamina != null && joyStamina.currentStamina <= 0f)
            {
                glideEnabled = false;
            }
            else
            {
                glideEnabled = true;

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);
            }
        }

        // ---------- ריחוף: כבידה חלשה + צריכת סטאמינה ----------
        if (glideEnabled)
        {
            if (joyStamina != null && !joyStamina.Use(glideCostPerSecond * Time.deltaTime))
            {
                glideEnabled = false;
                rb.gravityScale = normalGravity;
            }
            else
            {
                rb.gravityScale = glideGravity;
            }
        }
        else
        {
            rb.gravityScale = normalGravity;
        }

        // איפוס אירועי קלט – כדי שלא יופעלו שוב בפריים הבא
        jumpPressedThisFrame = false;
        jumpReleasedThisFrame = false;
    }

    // ------------ New Input System callbacks ------------

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // חייב להיות ככה כי האקשן נקרא Jump_Break
    public void OnJump_Break(InputValue value)
    {
        bool pressed = value.isPressed;

        if (pressed && !jumpHeld)
            jumpPressedThisFrame = true;

        if (!pressed && jumpHeld)
            jumpReleasedThisFrame = true;

        jumpHeld = pressed;
    }

    // Action בשם "Joy"
    public void OnJoy(InputValue value)
    {
        bool pressed = value.isPressed;

        if (joyToggleMode)
        {
            if (pressed)
                joyActive = !joyActive;
        }
        else
        {
            joyHeld = pressed;
        }
    }

    // Action בשם "Anger" – כרגע לא בשימוש (מנוהל ע"י EmotionController)
    public void OnAnger() { }

    // ---------------------------------------------------

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    Stamina GetStamina(Stamina.StaminaType wantedType)
    {
        Stamina[] staminas = GetComponents<Stamina>();
        foreach (Stamina s in staminas)
        {
            if (s.type == wantedType)
                return s;
        }
        return null;
    }
}
