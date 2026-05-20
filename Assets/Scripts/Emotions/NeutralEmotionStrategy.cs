using UnityEngine;

// אסטרטגיה של מצב ניטרלי:
// - תנועה ימינה/שמאלה בלבד
// - Space לא עושה כלום
// - אם נפסלים במצב ניטרלי -> Game Over מיידי
// - מעדכן אנימציית Idle / Walk לפי מהירות
public class NeutralEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Animation")]
    [SerializeField] private Animator neutralAnimator;

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;
    private Vector2 moveInput;

    // כדי שלא יופעל Game Over כמה פעמים
    private bool isFailing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();

        ResolveNeutralAnimator();
    }

    public void Enter()
    {
        isFailing = false;

        if (CanUseNeutralAnimator())
            neutralAnimator.SetFloat("speed", 0f);
    }

    public void Exit()
    {
        if (CanUseNeutralAnimator())
            neutralAnimator.SetFloat("speed", 0f);
    }

    public void HandleMove(Vector2 move)
    {
        if (isFailing) return;

        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // ניטרלי לא עושה כלום עם Space כרגע
    }

    public void Tick()
    {
        if (isFailing) return;

        if (hurtLock != null && hurtLock.IsLocked)
        {
            if (CanUseNeutralAnimator())
                neutralAnimator.SetFloat("speed", 0f);

            return;
        }

        float x = Mathf.Clamp(moveInput.x, -1f, 1f);

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (CanUseNeutralAnimator())
            neutralAnimator.SetFloat("speed", Mathf.Abs(x));
    }

    // במצב ניטרלי אין סטאמינה, אז כל פסילה היא Game Over מיידי
    public void HandleStaminaDepleted()
    {
        if (isFailing) return;

        isFailing = true;

        rb.linearVelocity = Vector2.zero;

        if (CanUseNeutralAnimator())
            neutralAnimator.SetFloat("speed", 0f);

        // במקום לחפש ישירות את PauseMenuInputSystem,
        // שולחים Event של Game Over
        GameEvents.RaiseGameOver();
    }

    private void ResolveNeutralAnimator()
    {
        if (neutralAnimator != null) return;

        Transform neutralVisual = transform.Find("NeutralVisual");

        if (neutralVisual != null)
            neutralAnimator = neutralVisual.GetComponent<Animator>();
    }

    private bool CanUseNeutralAnimator()
    {
        return neutralAnimator != null &&
               neutralAnimator.isActiveAndEnabled &&
               neutralAnimator.gameObject.activeInHierarchy &&
               neutralAnimator.runtimeAnimatorController != null;
    }
}