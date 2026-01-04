using UnityEngine;

public class PlayerJoyJump : MonoBehaviour
{
    public float moveSpeed = 6f;

    public float normalJumpForce = 3f;     // קפיצה רגילה
    public float joyJumpForce = 6f;        // קפיצה עם שמחה

    public bool joyActive = true;

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
    private Stamina stamina;

    private bool jumpedFromGround = false; // האם כבר קפצנו מהרצפה
    private bool glideEnabled = false;     // האם הריחוף הופעל (בלחיצה שנייה)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stamina = GetComponent<Stamina>();   // סטאמינה נמצאת על אותו Player
        rb.gravityScale = normalGravity;
    }

    void Update()
    {
        // תנועה
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        bool grounded = IsGrounded();

        // ✅ חדש: אם עזבנו את הרווח בזמן ריחוף — מפסיקים מיד ונופלים
        if (glideEnabled && Input.GetKeyUp(KeyCode.Space))
        {
            glideEnabled = false;
            rb.gravityScale = normalGravity;
        }

        // ✅ איפוס רק כשבאמת "נחתנו" (על הקרקע + לא עולים)
        if (grounded && rb.velocity.y <= 0.01f)
        {
            jumpedFromGround = false;
            glideEnabled = false;
            rb.gravityScale = normalGravity;
        }

        // לחיצה ראשונה: קפיצה (רק על הקרקע)
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            float jumpForce = joyActive ? joyJumpForce : normalJumpForce;

            jumpedFromGround = true;
            glideEnabled = false;

            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // לחיצה שנייה: מפעילה ריחוף + דחיפה למעלה (רק באוויר, פעם אחת בכל קפיצה)
        if (Input.GetKeyDown(KeyCode.Space) && !grounded && jumpedFromGround && !glideEnabled)
        {
            // אם אין סטאמינה - לא מתחילים ריחוף
            if (stamina != null && stamina.currentStamina <= 0f)
            {
                glideEnabled = false;
            }
            else
            {
                glideEnabled = true;

                // דחיפה למעלה
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);
            }
        }

        // ריחוף: אם הופעל בלחיצה השנייה — כבידה חלשה + צריכת סטאמינה
        if (glideEnabled)
        {
            // צורכים סטאמינה לפי זמן
            if (stamina != null && !stamina.Use(glideCostPerSecond * Time.deltaTime))
            {
                // נגמרה סטאמינה -> מפסיקים ריחוף
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"SPACE DOWN | grounded={grounded} | jumpedFromGround={jumpedFromGround} | glideEnabled={glideEnabled} | velY={rb.velocity.y}");
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }
}
