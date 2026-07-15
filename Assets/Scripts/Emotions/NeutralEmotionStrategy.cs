using UnityEngine;

// אסטרטגיה של מצב ניטרלי:
// - תנועה ימינה/שמאלה בלבד
// - Space לא עושה כלום
// - אם נפסלים במצב ניטרלי -> Game Over מיידי
// - מעדכן אנימציית Idle / Walk / Fall
public class NeutralEmotionStrategy : EmotionStrategyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Animation")]
    [SerializeField] private Animator neutralAnimator;

    protected override void Awake()
    {
        base.Awake();
        neutralAnimator = ResolveAnimator(neutralAnimator, "NeutralVisual");
    }

    public override void Enter()
    {
        IsFailing = false;
        UpdateAnimatorParams(neutralAnimator, 0f, Rb.linearVelocity.y, IsGrounded());
    }

    public override void Exit()
    {
        UpdateAnimatorParams(neutralAnimator, 0f, 0f, true);
    }

    public override void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // ניטרלי לא עושה כלום עם Space כרגע
    }

    public override void Tick()
    {
        if (IsFailing)
            return;

        bool grounded = IsGrounded();

        if (HurtLock != null && HurtLock.IsLocked)
        {
            UpdateAnimatorParams(neutralAnimator, 0f, Rb.linearVelocity.y, grounded);
            return;
        }

        float x = Mathf.Clamp(MoveInput.x, -1f, 1f);
        Rb.linearVelocity = new Vector2(x * moveSpeed, Rb.linearVelocity.y);

        UpdateAnimatorParams(neutralAnimator, Mathf.Abs(x), Rb.linearVelocity.y, grounded);
    }

    // במצב ניטרלי אין סטאמינה,
    // אז כל פסילה היא Game Over מיידי
    public override void HandleStaminaDepleted()
    {
        if (IsFailing)
            return;

        IsFailing = true;
        Rb.linearVelocity = Vector2.zero;

        UpdateAnimatorParams(neutralAnimator, 0f, 0f, IsGrounded());

        GameEvents.RaiseGameOver();
    }
}
