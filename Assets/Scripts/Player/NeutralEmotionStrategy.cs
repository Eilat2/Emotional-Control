using UnityEngine;

// אסטרטגיה של מצב ניטרלי:
// - תנועה ימינה/שמאלה בלבד
// - Space (Jump_Break) לא עושה כלום במצב ניטרלי
public class NeutralEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [SerializeField] float moveSpeed = 6f;

    private Rigidbody2D rb;              // פיזיקה של השחקן
    private PlayerHurtLock hurtLock;     // כדי לא לדרוס נוקבאק בזמן פגיעה
    private Vector2 moveInput;           // קלט תנועה מה-Context

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    // נקרא כשנכנסים למצב ניטרלי
    public void Enter()
    {
        // כרגע אין משהו מיוחד להפעיל
    }

    // נקרא כשעוזבים מצב ניטרלי
    public void Exit()
    {
        // כרגע אין משהו מיוחד לכבות
    }

    // קבלת תנועה מה-Context (OnMove)
    public void HandleMove(Vector2 move)
    {
        moveInput = move;
    }

    /*
     * קבלת Jump_Break מה-Context:
     * isHeld            - האם רווח מוחזק כרגע
     * pressedThisFrame  - האם נלחץ בפריים הזה
     * releasedThisFrame - האם שוחרר בפריים הזה
     *
     * בניטרלי אנחנו פשוט מתעלמים מהכפתור.
     */
    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // ניטרלי לא עושה כלום עם Space כרגע
    }

    // Tick נקרא כל פריים כשהאסטרטגיה פעילה
    public void Tick()
    {
        // אם השחקן בנעילת פגיעה - לא מזיזים אותו כדי לא לדרוס נוקבאק
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תנועה אופקית לפי הקלט
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
}
