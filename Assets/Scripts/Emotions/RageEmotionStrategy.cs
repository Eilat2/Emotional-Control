using UnityEngine;

// אסטרטגיה של מצב Rage:
// - תנועה ימינה/שמאלה
// - Space = תמיד מפעיל אנימציית שבירה
// - אם יש אובייקט שביר בטווח ויש סטאמינה -> שובר אחרי דיליי קטן
public class RageEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Break")]
    [SerializeField] private float breakCost = 20f;
    [SerializeField] private BreakableSensor sensor;

    [Header("Animation")]
    [SerializeField] private Animator rageAnimator;

    [Header("Break Timing")]
    [SerializeField] private float breakDelay = 0.2f;
    [SerializeField] private float breakAnimationLockTime = 0.35f;

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;
    private Stamina rageStamina;
    private Vector2 moveInput;

    // האם השחקן כרגע באמצע אנימציית שבירה
    private bool isBreaking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    private void Start()
    {
        rageStamina = GetStamina(Stamina.StaminaType.Rage);

        // אם לא חיברנו Animator ידנית, ננסה למצוא אותו בתוך RageVisual
        if (rageAnimator == null)
        {
            Transform rageVisual = transform.Find("RageVisual");

            if (rageVisual != null)
                rageAnimator = rageVisual.GetComponent<Animator>();
        }
    }

    public void Enter()
    {
        isBreaking = false;

        if (rageAnimator != null)
            rageAnimator.SetFloat("speed", 0f);
    }

    public void Exit()
    {
        isBreaking = false;

        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (rageAnimator != null)
            rageAnimator.SetFloat("speed", 0f);
    }

    public void HandleMove(Vector2 move)
    {
        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // מפעילים שבירה רק ברגע הלחיצה, לא בזמן החזקה
        if (!pressedThisFrame)
            return;

        // אם השחקן בנוקבאק / נעילת פגיעה - לא מאפשרים שבירה
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תמיד מפעילים אנימציית שבירה כשלוחצים Space במצב Rage
        PlayBreakAnimation();

        // אם אין חיישן או שאין כרגע אובייקט שביר בטווח -
        // רק האנימציה תתנגן ולא יישבר כלום
        if (sensor == null || sensor.current == null)
            return;

        // שומרים את האובייקט השביר ברגע הלחיצה.
        // זה חשוב כדי שגם אם החיישן משתנה בזמן הדיליי,
        // עדיין נשבור את מה שהשחקן התכוון לשבור ברגע הלחיצה.
        IBreakable targetToBreak = sensor.current;

        // אם אין מספיק סטאמינה - רק האנימציה תתנגן
        if (rageStamina != null && !rageStamina.Use(breakCost))
            return;

        // אם יש מטרה ויש סטאמינה - שוברים אחרי דיליי קטן
        StartCoroutine(BreakAfterDelay(targetToBreak));
    }

    public void Tick()
    {
        if (hurtLock != null && hurtLock.IsLocked)
        {
            if (rageAnimator != null)
                rageAnimator.SetFloat("speed", 0f);

            return;
        }

        float x = Mathf.Clamp(moveInput.x, -1f, 1f);

        // בזמן אנימציית שבירה עוצרים תנועה כדי שיראו את המכה
        if (isBreaking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (rageAnimator != null)
                rageAnimator.SetFloat("speed", 0f);

            return;
        }

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (rageAnimator != null)
            rageAnimator.SetFloat("speed", Mathf.Abs(x));
    }

    private void PlayBreakAnimation()
    {
        if (rageAnimator == null)
            return;

        StopCoroutine(nameof(BreakAnimationLock));
        StartCoroutine(nameof(BreakAnimationLock));

        rageAnimator.ResetTrigger("Break");
        rageAnimator.SetTrigger("Break");
    }

    private System.Collections.IEnumerator BreakAnimationLock()
    {
        isBreaking = true;

        yield return new WaitForSeconds(breakAnimationLockTime);

        isBreaking = false;
    }

    private System.Collections.IEnumerator BreakAfterDelay(IBreakable target)
    {
        yield return new WaitForSeconds(breakDelay);

        // אם עדיין קיימת מטרה אחרי הדיליי - שוברים אותה
        if (target != null)
        {
            Debug.Log("Breaking target: " + target);
            target.OnBreak();
        }
    }

    private Stamina GetStamina(Stamina.StaminaType wantedType)
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