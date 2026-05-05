using UnityEngine;
using System.Collections;

// אסטרטגיה של מצב Rage:
// - תנועה רגילה + שבירה
// - כשהסטאמינה נגמרת -> מאבד שליטה מצד לצד על הרצפה ואז Game Over
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

    [Header("Rage Failure (על הרצפה)")]
    [SerializeField] private float failureDuration = 2.5f;          // כמה זמן הזעם משתגע לפני Game Over
    [SerializeField] private float failureMoveSpeed = 8f;           // מהירות תנועה בזמן פסילה
    [SerializeField] private float failureShakeAmount = 0.10f;      // עוצמת רעידה בציר X
    [SerializeField] private float directionSwitchInterval = 0.12f; // כל כמה זמן מחליף כיוון

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;
    private Stamina rageStamina;
    private Vector2 moveInput;

    private bool isBreaking = false;
    private bool isFailing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    private void Start()
    {
        rageStamina = GetStamina(Stamina.StaminaType.Rage);
        ResolveRageAnimator();
    }

    public void Enter()
    {
        isBreaking = false;
        isFailing = false;

        if (CanUseRageAnimator())
            rageAnimator.SetFloat("speed", 0f);
    }

    public void Exit()
    {
        isBreaking = false;
        isFailing = false;

        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (CanUseRageAnimator())
            rageAnimator.SetFloat("speed", 0f);
    }

    public void HandleMove(Vector2 move)
    {
        if (isFailing) return;

        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        if (isFailing) return;
        if (!pressedThisFrame) return;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        PlayBreakAnimation();

        if (sensor == null || sensor.current == null)
            return;

        IBreakable targetToBreak = sensor.current;

        // אם אין סטאמינה -> מפעילים פסילה מיד
        if (rageStamina != null && !rageStamina.Use(breakCost))
        {
            HandleStaminaDepleted();
            return;
        }

        StartCoroutine(BreakAfterDelay(targetToBreak));
    }

    public void Tick()
    {
        if (isFailing) return;

        if (hurtLock != null && hurtLock.IsLocked)
        {
            if (CanUseRageAnimator())
                rageAnimator.SetFloat("speed", 0f);

            return;
        }

        float x = Mathf.Clamp(moveInput.x, -1f, 1f);

        // בזמן שבירה עוצרים תנועה כדי שיראו את המכה
        if (isBreaking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (CanUseRageAnimator())
                rageAnimator.SetFloat("speed", 0f);

            return;
        }

        // תנועה רגילה של Rage
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (CanUseRageAnimator())
            rageAnimator.SetFloat("speed", Mathf.Abs(x));
    }

    // נקרא כשהסטאמינה של Rage נגמרת
    public void HandleStaminaDepleted()
    {
        if (isFailing) return;

        isFailing = true;
        isBreaking = false;

        StartCoroutine(RageFailure());
    }

    private IEnumerator RageFailure()
    {
        float timer = 0f;

        // כיוון התנועה הראשוני
        float direction = 1f;

        // טיימר פנימי להחלפת כיוון
        float switchTimer = 0f;

        if (CanUseRageAnimator())
        {
            rageAnimator.SetFloat("speed", 0f);
            rageAnimator.ResetTrigger("Break");
        }

        // שומרים את הסקייל המקורי כדי להחזיר אותו בסוף
        Vector3 originalScale = transform.localScale;

        while (timer < failureDuration)
        {
            timer += Time.deltaTime;
            switchTimer += Time.deltaTime;

            // מחליפים כיוון בצורה חדה כל כמה רגעים
            if (switchTimer >= directionSwitchInterval)
            {
                direction *= -1f;
                switchTimer = 0f;
            }

            // תנועה חדה מצד לצד על הרצפה
            // שומרים על Y הנוכחי כדי שלא יקפוץ/ירחף
            rb.linearVelocity = new Vector2(
                direction * failureMoveSpeed * 3f,
                rb.linearVelocity.y
            );

            // רעידה רק בציר X כדי שישתגע לצדדים בלי לעלות לאוויר
            Vector3 shakeOffset = new Vector3(
                Random.Range(-failureShakeAmount * 4f, failureShakeAmount * 4f),
                0f,
                0f
            );

            transform.position += shakeOffset;

            // פולס בסקייל: מתרחב ונמחץ קצת, אבל נשאר על הרצפה
            float pulse = Mathf.Sin(Time.time * 18f);

            float xPulse = 1f + pulse * 0.18f;
            float yPulse = 1f - pulse * 0.08f;

            transform.localScale = new Vector3(
                originalScale.x * xPulse,
                originalScale.y * yPulse,
                originalScale.z
            );

            yield return null;
        }

        // מחזירים מצב תקין לפני פתיחת Game Over
        rb.linearVelocity = Vector2.zero;
        transform.localScale = originalScale;

        PauseMenuInputSystem pauseMenu =
            FindFirstObjectByType<PauseMenuInputSystem>(FindObjectsInactive.Include);

        if (pauseMenu != null)
        {
            pauseMenu.GameOver();
        }
        else
        {
            Debug.LogWarning("PauseMenuInputSystem not found in scene.");
        }
    }

    // מפעיל אנימציית שבירה
    private void PlayBreakAnimation()
    {
        if (!CanUseRageAnimator())
            return;

        StopCoroutine(nameof(BreakAnimationLock));
        StartCoroutine(nameof(BreakAnimationLock));

        rageAnimator.ResetTrigger("Break");
        rageAnimator.SetTrigger("Break");
    }

    // נועל תנועה לזמן קצר בזמן אנימציית השבירה
    private IEnumerator BreakAnimationLock()
    {
        isBreaking = true;

        yield return new WaitForSeconds(breakAnimationLockTime);

        isBreaking = false;
    }

    // שוברת את האובייקט אחרי דיליי קטן, כדי להתאים לאנימציה
    private IEnumerator BreakAfterDelay(IBreakable target)
    {
        yield return new WaitForSeconds(breakDelay);

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

    private void ResolveRageAnimator()
    {
        if (rageAnimator != null) return;

        Transform rageVisual = transform.Find("RageVisual");

        if (rageVisual != null)
            rageAnimator = rageVisual.GetComponent<Animator>();
    }

    private bool CanUseRageAnimator()
    {
        return rageAnimator != null &&
               rageAnimator.isActiveAndEnabled &&
               rageAnimator.gameObject.activeInHierarchy &&
               rageAnimator.runtimeAnimatorController != null;
    }
}