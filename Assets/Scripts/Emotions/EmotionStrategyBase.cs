using UnityEngine;

// ============================================================
//  EmotionStrategyBase – בסיס משותף לשלוש אסטרטגיות הרגש
//  (Joy / Rage / Neutral)
//
//  מרכז את מה ששלושתן עשו בנפרד ובאופן זהה כמעט לגמרי:
//    - caching של Rigidbody2D / PlayerHurtLock / PlayerStateMachine
//    - IsGrounded() (בדיקת groundCheck מול groundLayer)
//    - CanUseAnimator / UpdateAnimatorParams / ResolveAnimator
//    - guard של IsFailing + HandleMove (זהה בשלושתן)
//    - חיפוש קומפוננטת Stamina מהסוג הרצוי
//
//  כל אסטרטגיה עדיין שולטת לגמרי בלוגיקה הייחודית לה
//  (Enter/Exit/Tick/HandleJumpBreak/HandleStaminaDepleted
//  נשארים abstract).
// ============================================================
public abstract class EmotionStrategyBase : MonoBehaviour, IEmotionStrategy
{
    [Header("Ground Check")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundRadius = 0.25f;
    [SerializeField] protected LayerMask groundLayer;

    protected Rigidbody2D Rb { get; private set; }
    protected PlayerHurtLock HurtLock { get; private set; }
    protected PlayerStateMachine StateMachine { get; private set; }

    protected Vector2 MoveInput { get; set; }
    protected bool IsFailing { get; set; }

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        HurtLock = GetComponent<PlayerHurtLock>();
        StateMachine = GetComponent<PlayerStateMachine>();
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Tick();
    public abstract void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame);
    public abstract void HandleStaminaDepleted();

    public virtual void HandleMove(Vector2 move)
    {
        if (IsFailing)
            return;

        MoveInput = move;
    }

    /// <summary>
    /// בדיקת קרקע משותפת. כוללת null-check על groundCheck
    /// (בגרסה הקודמת של JoyEmotionStrategy הבדיקה הזו חסרה,
    /// מה שיכל לגרום ל-NullReferenceException אם שוכחים לשייך
    /// groundCheck ב-Inspector).
    /// </summary>
    protected bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    protected static bool CanUseAnimator(Animator animator)
    {
        return animator != null &&
               animator.isActiveAndEnabled &&
               animator.gameObject.activeInHierarchy &&
               animator.runtimeAnimatorController != null;
    }

    /// <summary>
    /// מעדכן את שלושת הפרמטרים המשותפים לכל האנימטורים
    /// (speed / yVelocity / isGrounded). אם לאסטרטגיה יש עוד
    /// פרמטר ייחודי (כמו isGliding אצל Joy) - להוסיף אותו בנפרד.
    /// </summary>
    protected static void UpdateAnimatorParams(Animator animator, float speed, float yVelocity, bool grounded)
    {
        if (!CanUseAnimator(animator))
            return;

        animator.SetFloat("speed", speed);
        animator.SetFloat("yVelocity", yVelocity);
        animator.SetBool("isGrounded", grounded);
    }

    /// <summary>
    /// אם השדה כבר מולא ב-Inspector - משאיר כמו שהוא.
    /// אחרת מחפש Transform בשם visualChildName ומנסה למצוא עליו Animator.
    /// </summary>
    protected Animator ResolveAnimator(Animator existing, string visualChildName)
    {
        if (existing != null)
            return existing;

        Transform visual = transform.Find(visualChildName);
        return visual != null ? visual.GetComponent<Animator>() : null;
    }

    protected Stamina FindStamina(Stamina.StaminaType type, bool includeChildren)
    {
        Stamina[] staminas = includeChildren
            ? GetComponentsInChildren<Stamina>()
            : GetComponents<Stamina>();

        foreach (Stamina s in staminas)
        {
            if (s.type == type)
                return s;
        }

        return null;
    }
}
