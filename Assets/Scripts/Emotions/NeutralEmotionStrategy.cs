using UnityEngine;

// אסטרטגיה של מצב ניטרלי:
// - תנועה ימינה/שמאלה בלבד
// - Space לא עושה כלום
// - מעדכן אנימציית Idle / Walk לפי מהירות
public class NeutralEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Animation")]
    [SerializeField] private Animator neutralAnimator; // לגרור לכאן את Animator של NeutralVisual

    private Rigidbody2D rb;              // פיזיקה של השחקן
    private PlayerHurtLock hurtLock;     // כדי לא לדרוס נוקבאק בזמן פגיעה
    private Vector2 moveInput;           // קלט תנועה מה-Context

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();

        // אם לא חיברת ידנית, ננסה למצוא את ה-Animator של NeutralVisual
        ResolveNeutralAnimator();
    }

    public void Enter()
    {
        // כשנכנסים לניטרלי מתחילים ממצב עמידה
        if (neutralAnimator != null)
            neutralAnimator.SetFloat("speed", 0f);
    }

    public void Exit()
    {
        // כשעוזבים ניטרלי מאפסים את האנימציה
        if (neutralAnimator != null)
            neutralAnimator.SetFloat("speed", 0f);
    }

    public void HandleMove(Vector2 move)
    {
        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // ניטרלי לא עושה כלום עם Space כרגע
    }

    public void Tick()
    {
        // אם השחקן בנעילת פגיעה - לא מזיזים אותו כדי לא לדרוס נוקבאק
        if (hurtLock != null && hurtLock.IsLocked)
        {
            if (neutralAnimator != null)
                neutralAnimator.SetFloat("speed", 0f);

            return;
        }

        float x = Mathf.Clamp(moveInput.x, -1f, 1f);

        // תנועה אופקית לפי הקלט
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // עדכון אנימציית Idle / Walk
        if (neutralAnimator != null)
            neutralAnimator.SetFloat("speed", Mathf.Abs(x));
    }

    // מחפש את ה-Animator של NeutralVisual אם לא חיברת ידנית באינספקטור
    private void ResolveNeutralAnimator()
    {
        if (neutralAnimator != null) return;

        Transform neutralVisual = transform.Find("NeutralVisual");

        if (neutralVisual != null)
            neutralAnimator = neutralVisual.GetComponent<Animator>();
    }
}