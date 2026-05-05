using UnityEngine;

public class Stamina : MonoBehaviour
{
    // סוג הסטאמינה – כדי לדעת אם שייכת ל-Joy או Rage
    public enum StaminaType
    {
        Joy,
        Rage
    }

    // סוג הסטאמינה של האובייקט הזה (נקבע באינספקטור)
    public StaminaType type = StaminaType.Joy;

    // מקסימום סטאמינה
    public float maxStamina = 100f;

    // סטאמינה נוכחית בזמן המשחק
    public float currentStamina;

    [Header("טעינה (Regen)")]
    public float regenPerSecond = 1f;   // כמה נטען כל שנייה
    public float regenDelay = 0.4f;     // דיליי אחרי שימוש לפני טעינה

    [Header("זיהוי חוסר תזוזה")]
    public float idleSpeedThreshold = 0.05f;

    private Rigidbody2D rb;                      // כדי לבדוק אם השחקן זז
    private float lastUseTime;                   // מתי השתמשנו לאחרונה
    private PlayerEmotionContext emotionContext; // 🔥 חיבור ל-Context

    // כדי שלא נפעיל GameOver כמה פעמים
    private bool depletedTriggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // מביאים את ה-Context מהשחקן
        emotionContext = GetComponent<PlayerEmotionContext>();
    }

    void Start()
    {
        // מתחילים עם סטאמינה מלאה
        currentStamina = maxStamina;
    }

    void Update()
    {
        // טעינה אוטומטית רק כשהשחקן לא זז
        RegenerateWhenIdle();
    }

    void RegenerateWhenIdle()
    {
        // אם כבר מלא – אין צורך להטעין
        if (currentStamina >= maxStamina)
            return;

        // אם עוד לא עבר זמן מאז השימוש האחרון – לא נטען
        if (Time.time < lastUseTime + regenDelay)
            return;

        // אם השחקן זז – לא נטען
        if (!IsIdle())
            return;

        // טעינה הדרגתית
        currentStamina += regenPerSecond * Time.deltaTime;

        // שלא יעבור את המקסימום
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    bool IsIdle()
    {
        // אם אין Rigidbody – נניח שהוא idle
        if (rb == null) return true;

        // בודקים מהירות
        return rb.linearVelocity.magnitude <= idleSpeedThreshold;
    }

    /// <summary>
    /// שימוש בסטאמינה
    /// מחזיר true אם נשארה סטאמינה
    /// מחזיר false אם נגמרה
    /// </summary>
    public bool Use(float amount)
    {
        // אם כבר אין סטאמינה – מפעילים כישלון
        if (currentStamina <= 0f)
        {
            TriggerDepleted();
            return false;
        }

        // מורידים סטאמינה
        currentStamina -= amount;

        // לא יורדים מתחת ל-0
        currentStamina = Mathf.Max(currentStamina, 0f);

        // שומרים זמן שימוש אחרון
        lastUseTime = Time.time;

        // אם נגמר עכשיו – מפעילים כישלון
        if (currentStamina <= 0f)
        {
            TriggerDepleted();
            return false;
        }

        return true;
    }

    /// <summary>
    /// מפעיל את ה-Failure של השחקן (דרך ה-Context)
    /// </summary>
    private void TriggerDepleted()
    {
        // שלא יקרה פעמיים
        if (depletedTriggered)
            return;

        depletedTriggered = true;

        // קוראים ל-Context שיפעיל את ה-Strategy המתאים
        if (emotionContext != null)
        {
            emotionContext.OnStaminaDepleted();
        }
    }

    /// <summary>
    /// מילוי מלא (למשל אחרי Respawn)
    /// </summary>
    public void Refill()
    {
        currentStamina = maxStamina;
        depletedTriggered = false;
    }

    /// <summary>
    /// איפוס כשעוברים סצנה
    /// </summary>
    public void ResetForNewScene()
    {
        currentStamina = maxStamina;
        lastUseTime = 0f;
        depletedTriggered = false;
    }
}