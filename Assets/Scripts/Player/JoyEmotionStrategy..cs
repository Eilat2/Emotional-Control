using UnityEngine;

public class JoyEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float joyJumpForce = 6f;

    [Header("Glide (2nd press)")]
    [SerializeField] private float normalGravity = 4f;
    [SerializeField] private float glideGravity = 1.2f;
    [SerializeField] private float floatUpImpulse = 8f;

    [Header("Stamina (Joy)")]
    [SerializeField] private float glideCostPerSecond = 15f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Input stability")]
    [SerializeField] private float holdGraceSeconds = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    // ✅ אופציה הכי יציבה: אם תגררי לפה את ה-Stamina(type=Joy) מה-Player – אין יותר בעיות חיפוש
    [SerializeField] private Stamina joyStamina;

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;

    private Vector2 moveInput;

    private bool jumpedFromGround = false; // כבר הייתה קפיצה ראשונה
    private bool glideEnabled = false;     // מצב ריחוף “מוכן” אחרי לחיצה שנייה

    private bool jumpHeld = false;         // האם Space מוחזק
    private float holdGraceTimer = 0f;     // “חסד” קצר כדי למנוע נפילת held בפריים

    // כדי שלא נציף את הקונסול באותה שגיאה
    private bool staminaErrorLogged = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    void Start()
    {
        rb.gravityScale = normalGravity;

        // ניסיון ראשון למצוא Stamina אם לא גררת ידנית
        ResolveJoyStamina();
    }

    public void Enter()
    {
        rb.gravityScale = normalGravity;

        // לפעמים נכנסים לרגש אחרי שדברים נטענו – אז ננסה שוב
        ResolveJoyStamina();

        if (debugLogs) Debug.Log("[JOY] Enter");
    }

    public void Exit()
    {
        // כשעוזבים Joy – להחזיר כבידה רגילה ולא להשאיר מצב ריחוף דולק
        glideEnabled = false;
        holdGraceTimer = 0f;
        rb.gravityScale = normalGravity;

        if (debugLogs) Debug.Log("[JOY] Exit");
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

        // איפוס מלא רק כשבאמת נחתנו
        if (grounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpedFromGround = false;
            glideEnabled = false;
            holdGraceTimer = 0f;
            rb.gravityScale = normalGravity;
        }

        // שחרור רווח בזמן ריחוף -> מפסיקים מיד
        if (glideEnabled && releasedThisFrame)
        {
            glideEnabled = false;
            holdGraceTimer = 0f;
            rb.gravityScale = normalGravity;

            if (debugLogs) Debug.Log("[JOY] Glide stopped (released)");
            return;
        }

        // ---------- לחיצה ראשונה: קפיצה מהקרקע ----------
        if (pressedThisFrame && grounded)
        {
            jumpedFromGround = true;
            glideEnabled = false;
            holdGraceTimer = 0f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * joyJumpForce, ForceMode2D.Impulse);

            if (debugLogs) Debug.Log("[JOY] First jump");
            return;
        }

        // ---------- לחיצה שנייה: “הדלקת מצב ריחוף” ----------
        if (pressedThisFrame && !grounded && jumpedFromGround && !glideEnabled)
        {
            // נוודא שיש לנו stamina (או ננסה למצוא שוב)
            ResolveJoyStamina();

            // אם אין סטאמינה/נגמרה – לא נכנסים לריחוף
            if (joyStamina != null && joyStamina.currentStamina <= 0f)
            {
                if (debugLogs) Debug.Log("[JOY] Glide blocked (no stamina)");
                return;
            }

            glideEnabled = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);

            holdGraceTimer = holdGraceSeconds;

            if (debugLogs) Debug.Log("[JOY] Glide activated (2nd press)");
        }
    }

    public void Tick()
    {
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תנועה אופקית
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // אם לא במצב glide בכלל -> כבידה רגילה
        if (!glideEnabled)
        {
            rb.gravityScale = normalGravity;
            return;
        }

        // מנגנון hold עם grace
        if (jumpHeld)
            holdGraceTimer = holdGraceSeconds;
        else
            holdGraceTimer -= Time.deltaTime;

        bool treatAsHeld = jumpHeld || holdGraceTimer > 0f;

        // ✅ מה שביקשת: ירידת סטאמינה הדרגתית רק בזמן ריחוף בפועל (כל עוד מוחזק)
        if (treatAsHeld)
        {
            // שוב, ליתר ביטחון – אם פתאום נהיה null (prefab/instantiate מוזר), ננסה לפתור
            ResolveJoyStamina();

            if (joyStamina == null)
            {
                // פה בדיוק אצלך זה קורה כרגע
                if (!staminaErrorLogged)
                {
                    Debug.LogError("[JOY] joyStamina == null בזמן ריחוף. גררי ידנית את Stamina(type=Joy) לשדה joyStamina בסקריפט JoyEmotionStrategy, או ודאי שהיא על אותו Player שמריץ את האסטרטגיה.");
                    staminaErrorLogged = true;
                }

                // עדיין נרחף מבחינת פיזיקה, אבל בלי ירידה כי אין רפרנס
                rb.gravityScale = glideGravity;
                return;
            }

            // מורידים סטאמינה פר-שנייה * זמן פריים = ירידה הדרגתית
            float cost = glideCostPerSecond * Time.deltaTime;

            if (debugLogs) Debug.Log($"[JOY] Gliding cost={cost:F3}, stamina before={joyStamina.currentStamina:F1}");

            if (!joyStamina.Use(cost))
            {
                // נגמרה סטאמינה -> מפסיקים ריחוף
                glideEnabled = false;
                holdGraceTimer = 0f;
                rb.gravityScale = normalGravity;

                if (debugLogs) Debug.Log("[JOY] Stamina finished – glide stopped");
                return;
            }

            rb.gravityScale = glideGravity;
        }
        else
        {
            // לא מוחזק -> לא מרחפים בפועל -> כבידה רגילה + אין ירידת סטאמינה
            rb.gravityScale = normalGravity;
        }
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

    /// <summary>
    /// מנסה למצוא Stamina מסוג Joy בצורה סופר-בטוחה.
    /// אם גררת ידנית -> לא עושה כלום.
    /// אם לא -> מחפש על אותו GameObject, על ההורה/שורש, ובילדים.
    /// </summary>
    private void ResolveJoyStamina()
    {
        if (joyStamina != null) return;

        // 1) הכי הגיוני (כמו שהיה בקוד הישן שעבד): על אותו Player
        Stamina[] local = GetComponents<Stamina>();
        foreach (var s in local)
        {
            if (s.type == Stamina.StaminaType.Joy)
            {
                joyStamina = s;
                staminaErrorLogged = false;
                if (debugLogs) Debug.Log($"[JOY] Found stamina locally on {gameObject.name}");
                return;
            }
        }

        // 2) על ההורה/שורש + ילדים (כולל disabled)
        Transform root = transform.root;
        Stamina[] all = root.GetComponentsInChildren<Stamina>(true);
        foreach (var s in all)
        {
            if (s.type == Stamina.StaminaType.Joy)
            {
                joyStamina = s;
                staminaErrorLogged = false;
                if (debugLogs) Debug.Log($"[JOY] Found stamina in hierarchy on {s.gameObject.name}");
                return;
            }
        }

        // אם לא מצאנו – נשאר null (ואז תראי שגיאה פעם אחת בזמן glide)
    }
}
