using UnityEngine;

public class Stamina : MonoBehaviour
{
    // סוג הסטאמינה – כדי שנדע אם היא שייכת לשמחה או לזעם
    public enum StaminaType
    {
        Joy,
        Rage
    }

    // קובעים באינספקטור האם זו סטאמינה של Joy או Rage
    public StaminaType type = StaminaType.Joy;

    // כמות הסטאמינה המקסימלית
    public float maxStamina = 100f;

    // כמות הסטאמינה הנוכחית (משתנה בזמן המשחק)
    public float currentStamina;

    [Header("טעינה (Regen)")]
    public float regenPerSecond = 1f;           // כמה סטאמינה נטענת בשנייה
    public float regenDelay = 0.4f;             // כמה זמן אחרי שימוש מתחילים להיטען

    [Header("זיהוי חוסר תזוזה")]
    public float idleSpeedThreshold = 0.05f;    // מתחת לזה נחשב "לא זז"

    private Rigidbody2D rb;                     // כדי לדעת אם השחקן זז
    private float lastUseTime;                  // מתי בפעם האחרונה השתמשנו בסטאמינה

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // בתחילת המשחק הסטאמינה מלאה
        currentStamina = maxStamina;
    }

    void Update()
    {
        // טעינה אוטומטית רק כשהשחקן לא זז
        RegenerateWhenIdle();
    }

    void RegenerateWhenIdle()
    {
        // אם כבר מלא – אין מה להטעין
        if (currentStamina >= maxStamina)
            return;

        // אם עדיין לא עבר זמן הדיליי מאז שימוש – לא נטען עדיין
        if (Time.time < lastUseTime + regenDelay)
            return;

        // אם השחקן זז – לא נטען
        if (!IsIdle())
            return;

        // נטענים לאט לאט
        currentStamina += regenPerSecond * Time.deltaTime;

        // דואגים שלא יעלה מעל המקסימום
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    bool IsIdle()
    {
        // אם אין Rigidbody2D (לא אמור לקרות) אז נתייחס כ-idle
        if (rb == null) return true;

        // נבדוק מהירות בפועל
        return rb.linearVelocity.magnitude <= idleSpeedThreshold;
    }

    /// <summary>
    /// שימוש בסטאמינה
    /// amount = כמה להוריד
    /// מחזיר true אם עדיין נשארה סטאמינה, false אם נגמרה
    /// </summary>
    public bool Use(float amount)
    {
        // אם כבר אין סטאמינה – אי אפשר להשתמש
        if (currentStamina <= 0f)
            return false;

        // מורידים את הכמות שביקשו
        currentStamina -= amount;

        // דואגים שלא תרד מתחת ל-0
        currentStamina = Mathf.Max(currentStamina, 0f);

        // חשוב: מסמנים זמן שימוש אחרון כדי לעצור טעינה רגע אחרי שימוש
        lastUseTime = Time.time;

        // מחזיר האם עדיין יש סטאמינה
        return currentStamina > 0f;
    }

    /// <summary>
    /// מילוי מלא של הסטאמינה
    /// (לדוגמה: אחרי פסילה / Respawn)
    /// </summary>
    public void Refill()
    {
        currentStamina = maxStamina;
    }
}
