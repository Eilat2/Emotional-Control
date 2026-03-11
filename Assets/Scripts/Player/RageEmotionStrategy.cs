using UnityEngine;

// אסטרטגיה של מצב Rage:
// - תנועה ימינה/שמאלה
// - Space (Jump_Break) = שבירה אם יש אובייקט שביר בטווח (BreakableSensor)
// - שבירה מורידה סטאמינה של Rage בלבד
public class RageEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;   // מהירות תנועה של Rage (תשני באינספקטור אם צריך)

    [Header("Break")]
    [SerializeField] float breakCost = 20f;  // כמה סטאמינה יורדת על שבירה
    [SerializeField] BreakableSensor sensor; // לגרור לכאן את BreakZone (הילד) שיש עליו BreakableSensor

    private Rigidbody2D rb;                  // פיזיקה של השחקן
    private PlayerHurtLock hurtLock;         // נעילת פגיעה (Knockback)
    private Stamina rageStamina;             // סטאמינה של Rage בלבד
    private Vector2 moveInput;               // קלט תנועה מה-Context

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    void Start()
    {
        // מוצאים את קומפוננטת הסטאמינה מסוג Rage (יש לך שתי קומפוננטות Stamina על השחקן)
        rageStamina = GetStamina(Stamina.StaminaType.Rage);
    }

    public void Enter()
    {
        // נקרא כשנכנסים לרגש (כרגע אין משהו מיוחד)
    }

    public void Exit()
    {
        // כשיוצאים מהרגש - מאפסים מהירות אופקית כדי שלא "יסחב" תנועה לרגש הבא
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    // קבלת תנועה מה-Context
    public void HandleMove(Vector2 move)
    {
        moveInput = move;
    }

    /*
     * קבלת Jump_Break מה-Context:
     * אנחנו שוברים רק על לחיצה (pressedThisFrame) ולא על החזקה,
     * כדי שלא ישבור מלא פעמים ברצף.
     */
    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        if (!pressedThisFrame)
            return;

        // אם השחקן בנוקבאק - לא מבצעים שבירה
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // אם אין סנסור או אין אובייקט שביר בטווח - אין מה לשבור
        if (sensor == null || sensor.current == null)
            return;

        // אם אין מספיק סטאמינה - לא מבצעים שבירה
        if (rageStamina != null && !rageStamina.Use(breakCost))
            return;

        // מבצעים שבירה
        sensor.current.OnBreak();
    }

    // Tick רץ ב-FixedUpdate דרך PlayerEmotionContext (אחרי התיקון שעשינו שם)
    public void Tick()
    {
        // אם השחקן בפגיעה - לא מזיזים כדי לא לדרוס נוקבאק
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // קלט אופקי אמור להיות בטווח -1..1 (ימינה/שמאלה)
        float x = Mathf.Clamp(moveInput.x, -1f, 1f);

        // תנועה אופקית לפי מהירות Rage
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    // חיפוש קומפוננטת סטאמינה לפי סוג (Joy / Rage)
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
