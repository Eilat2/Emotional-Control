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

    [Header("Float Up (2nd press)")]
    public float floatUpImpulse = 8f;      // דחיפה למעלה בלחיצה השנייה

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private bool jumpedFromGround = false; // האם כבר קפצנו מהרצפה
    private bool glideEnabled = false;     // האם הריחוף הופעל (בלחיצה שנייה)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            glideEnabled = true;

            // דחיפה למעלה (כדי שזה יהיה "מרחף כלפי מעלה" ולא רק נופל לאט)
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);
        }

        // ריחוף: אם הופעל בלחיצה השנייה — כבידה חלשה
        if (glideEnabled)
        {
            rb.gravityScale = glideGravity;
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
