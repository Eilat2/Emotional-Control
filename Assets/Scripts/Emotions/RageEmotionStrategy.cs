using UnityEngine;
using System.Collections;

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
    [SerializeField] private float breakAnimationLockTime = 0.35f;

    [Header("Rage Failure (על הרצפה)")]
    [SerializeField] private float failureDuration = 2.5f;
    [SerializeField] private float failureMoveSpeed = 8f;
    [SerializeField] private float failureShakeAmount = 0.10f;
    [SerializeField] private float directionSwitchInterval = 0.12f;

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;
    private PlayerStateMachine stateMachine;
    private Stamina rageStamina;

    private Vector2 moveInput;

    private bool isBreaking = false;
    private bool isFailing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
        stateMachine = GetComponent<PlayerStateMachine>();
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
        if (isFailing)
            return;

        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        if (isFailing)
            return;

        if (!pressedThisFrame)
            return;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        PlayBreakAnimation();

        if (sensor == null || sensor.current == null)
            return;

        IBreakable targetToBreak = sensor.current;

        if (rageStamina != null && !rageStamina.Use(breakCost))
        {
            HandleStaminaDepleted();
            return;
        }

        if (stateMachine != null)
        {
            stateMachine.TriggerBreak(targetToBreak);
        }
        else
        {
            targetToBreak.OnBreak();
        }
    }

    public void Tick()
    {
        if (isFailing)
            return;

        if (hurtLock != null && hurtLock.IsLocked)
        {
            if (CanUseRageAnimator())
                rageAnimator.SetFloat("speed", 0f);

            return;
        }

        float x = Mathf.Clamp(moveInput.x, -1f, 1f);

        if (isBreaking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (CanUseRageAnimator())
                rageAnimator.SetFloat("speed", 0f);

            return;
        }

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (CanUseRageAnimator())
            rageAnimator.SetFloat("speed", Mathf.Abs(x));
    }

    public void HandleStaminaDepleted()
    {
        if (isFailing)
            return;

        isFailing = true;
        isBreaking = false;

        StartCoroutine(RageFailure());
    }

    private IEnumerator RageFailure()
    {
        float timer = 0f;
        float direction = 1f;
        float switchTimer = 0f;

        if (CanUseRageAnimator())
        {
            rageAnimator.SetFloat("speed", 0f);
            rageAnimator.ResetTrigger("Break");
        }

        Vector3 originalScale = transform.localScale;

        while (timer < failureDuration)
        {
            timer += Time.deltaTime;
            switchTimer += Time.deltaTime;

            if (switchTimer >= directionSwitchInterval)
            {
                direction *= -1f;
                switchTimer = 0f;
            }

            rb.linearVelocity = new Vector2(
                direction * failureMoveSpeed * 3f,
                rb.linearVelocity.y
            );

            Vector3 shakeOffset = new Vector3(
                Random.Range(-failureShakeAmount * 4f, failureShakeAmount * 4f),
                0f,
                0f
            );

            transform.position += shakeOffset;

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

        rb.linearVelocity = Vector2.zero;
        transform.localScale = originalScale;

        GameEvents.RaiseGameOver();
    }

    private void PlayBreakAnimation()
    {
        if (!CanUseRageAnimator())
            return;

        StopCoroutine(nameof(BreakAnimationLock));
        StartCoroutine(nameof(BreakAnimationLock));

        rageAnimator.ResetTrigger("Break");
        rageAnimator.SetTrigger("Break");
    }

    private IEnumerator BreakAnimationLock()
    {
        isBreaking = true;

        yield return new WaitForSeconds(breakAnimationLockTime);

        isBreaking = false;
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
        if (rageAnimator != null)
            return;

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