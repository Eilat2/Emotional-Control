using UnityEngine;
using UnityEngine.InputSystem; // חשוב! New Input System

public class PlayerJoyJump : MonoBehaviour
{
    public float moveSpeed = 6f;

    public float normalJumpForce = 3f;     // קפיצה רגילה
    public float joyJumpForce = 6f;        // קפיצה עם שמחה

    [Header("Joy Mode")]
    public bool joyActive = true;

    // אם true: לחיצה אחת מדליקה/מכבה שמחה (Toggle)
    // אם false: שמחה פעילה רק בזמן שמחזיקים את הכפתור
    public bool joyToggleMode = true;

    [Header("Glide (Joy)")]
    public float normalGravity = 4f;
    public float glideGravity = 1.2f;

    [Header("Stamina (Joy)")]
    public float glideCostPerSecond = 15f; // כמה סטאמינה יורדת בשנייה בזמן ריחוף

    [Header("Float Up (2nd press)")]
    public float floatUpImpulse = 8f;      // דחיפה למעלה בלחיצה השנייה

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    // סטאמינה של שמחה בלבד (Joy)
    private Stamina joyStamina;

    private bool jumpedFromGround = false; // האם כבר קפצנו מהרצפה
    private bool glideEnabled = false;     // האם הריחוף הופעל (בלחיצה שנייה)

    // ----------- New Input System state -----------
    private Vector2 moveInput;
    private bool jumpHeld = false;
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
        // ---------------- תנועה ----------------
        float move = moveInput.x;
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        bool grounded = IsGrounded();

        // אם Joy במצב "החזקה" - joyActive נקבע לפי האם הכפתור מוחזק
        if (!joyToggleMode)
            joyActive = joyHeld;

        // אם שחררנו את כפתור הקפיצה בזמן ריחוף — מפסיקים מיד ונופלים
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

        // ---------------- לחיצה ראשונה: קפיצה (רק על הקרקע) ----------------
        if (jumpPressedThisFrame && grounded)
        {
            float jumpForce = joyActive ? joyJumpForce : normalJumpForce;

            jumpedFromGround = true;
            glideEnabled = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // ---------------- לחיצה שנייה: ריחוף + דחיפה למעלה ----------------
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

        // ---------------- ריחוף: כבידה חלשה + צריכת סטאמינה ----------------
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

        // דיבאג
        if (jumpPressedThisFrame)
        {
            Debug.Log($"JUMP DOWN | grounded={grounded} | jumpedFromGround={jumpedFromGround} | glideEnabled={glideEnabled} | velY={rb.linearVelocity.y} | joyActive={joyActive}");
        }

        // מאפסים "יריות"
        jumpPressedThisFrame = false;
        jumpReleasedThisFrame = false;
    }

    // ------------ New Input System callbacks (Send Messages) ------------

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // ✅ חייב להיות ככה כי האקשן נקרא Jump_Break
    public void OnJump_Break(InputValue value)
    {
        bool pressed = value.isPressed;

        if (pressed && !jumpHeld)
            jumpPressedThisFrame = true;

        if (!pressed && jumpHeld)
            jumpReleasedThisFrame = true;

        jumpHeld = pressed;
    }

    // Action בשם "Joy" יפעיל את זה
    public void OnJoy(InputValue value)
    {
        bool pressed = value.isPressed;

        if (joyToggleMode)
        {
            // Toggle רק על "Down"
            if (pressed)
            {
                joyActive = !joyActive;
                Debug.Log("JOY TOGGLE: " + joyActive);
            }
        }
        else
        {
            // מצב החזקה
            joyHeld = pressed;
        }
    }

    // Action בשם "Anger" יפעיל את זה
    public void OnAnger()
    {
        Debug.Log("ANGER pressed!");
        // פה נחבר את ה"גשר" / כוח זעם כשתגידי מה בדיוק צריך לעשות
    }

    // -------------------------------------------------------------------

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