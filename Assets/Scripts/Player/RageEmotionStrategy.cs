using UnityEngine;

// אסטרטגיה של מצב Rage:
// - תנועה ימינה/שמאלה
// - Space (Jump_Break) = שבירה אם יש אובייקט שביר בטווח (BreakableSensor)
// - שבירה עולה סטאמינה של Rage בלבד
public class RageEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [SerializeField] float moveSpeed = 6f;

    [Header("Break")]
    [SerializeField] float breakCost = 20f;         // כמה סטאמינה יורדת על שבירה
    [SerializeField] BreakableSensor sensor;        // לגרור את BreakZone (child) שיש עליו BreakableSensor

    private Rigidbody2D rb;                         // פיזיקה של השחקן
    private PlayerHurtLock hurtLock;                // נעילת פגיעה
    private Stamina rageStamina;                    // סטאמינה של Rage בלבד
    private Vector2 moveInput;                      // קלט תנועה מה-Context

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    void Start()
    {
        // מוצאים סטאמינה לפי סוג Rage (יש לך 2 קומפוננטות Stamina על השחקן)
        rageStamina = GetStamina(Stamina.StaminaType.Rage);
    }

    public void Enter()
    {
        // אין משהו מיוחד כרגע
    }

    public void Exit()
    {
        // אין משהו מיוחד כרגע
    }

    // קבלת תנועה מה-Context
    public void HandleMove(Vector2 move)
    {
        moveInput = move;
    }

    /*
     * קבלת Jump_Break מה-Context:
     * חשוב: אנחנו רוצים לבצע שבירה רק על "לחיצה" (pressedThisFrame),
     * ולא כל עוד מחזיקים את הכפתור (isHeld), כדי שלא ישבור 100 פעמים.
     */
    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // אם לא נלחץ עכשיו (Down) - לא עושים כלום
        if (!pressedThisFrame)
            return;

        // אם השחקן בנוקבאק - לא מבצעים שבירה
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // אם אין סנסור או אין משהו שביר בטווח - אין מה לשבור
        if (sensor == null || sensor.current == null)
            return;

        // אם אין מספיק סטאמינה - לא מבצעים
        if (rageStamina != null && !rageStamina.Use(breakCost))
            return;

        // מבצעים שבירה: אומרים לאובייקט השביר לטפל בעצמו
        sensor.current.OnBreak();
    }

    // Tick נקרא כל פריים כשהאסטרטגיה פעילה
    public void Tick()
    {
        // אם השחקן בפגיעה - לא מזיזים כדי לא לדרוס נוקבאק
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תנועה אופקית
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
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
