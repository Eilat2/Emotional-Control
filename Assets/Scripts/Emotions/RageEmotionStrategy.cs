using UnityEngine;
using System.Collections;

public class RageEmotionStrategy : EmotionStrategyBase
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

    [Header("Rage Failure")]
    [SerializeField] private float failureDuration = 2.5f;
    [SerializeField] private float failureMoveSpeed = 8f;
    [SerializeField] private float failureShakeAmount = 0.10f;
    [SerializeField] private float directionSwitchInterval = 0.12f;

    private Stamina _rageStamina;
    private bool _isBreaking;

    private void Start()
    {
        if (_rageStamina == null)
            _rageStamina = FindStamina(Stamina.StaminaType.Rage, includeChildren: false);

        rageAnimator = ResolveAnimator(rageAnimator, "RageVisual");
    }

    public override void Enter()
    {
        _isBreaking = false;
        IsFailing = false;

        UpdateAnimatorParams(rageAnimator, 0f, Rb.linearVelocity.y, IsGrounded());
    }

    public override void Exit()
    {
        _isBreaking = false;
        IsFailing = false;

        if (Rb != null)
            Rb.linearVelocity = new Vector2(0f, Rb.linearVelocity.y);

        UpdateAnimatorParams(rageAnimator, 0f, 0f, true);
    }

    public override void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        if (IsFailing || !pressedThisFrame)
            return;

        if (HurtLock != null && HurtLock.IsLocked)
            return;

        PlayBreakAnimation();

        if (sensor == null || sensor.current == null)
            return;

        IBreakable targetToBreak = sensor.current;

        if (_rageStamina != null && !_rageStamina.Use(breakCost))
        {
            HandleStaminaDepleted();
            return;
        }

        if (StateMachine != null)
            StateMachine.TriggerBreak(targetToBreak);
        else
            targetToBreak.OnBreak();
    }

    public override void Tick()
    {
        if (IsFailing)
            return;

        bool grounded = IsGrounded();

        if (HurtLock != null && HurtLock.IsLocked)
        {
            UpdateAnimatorParams(rageAnimator, 0f, Rb.linearVelocity.y, grounded);
            return;
        }

        float x = Mathf.Clamp(MoveInput.x, -1f, 1f);

        if (_isBreaking)
        {
            Rb.linearVelocity = new Vector2(0f, Rb.linearVelocity.y);
            UpdateAnimatorParams(rageAnimator, 0f, Rb.linearVelocity.y, grounded);
            return;
        }

        Rb.linearVelocity = new Vector2(x * moveSpeed, Rb.linearVelocity.y);
        UpdateAnimatorParams(rageAnimator, Mathf.Abs(x), Rb.linearVelocity.y, grounded);
    }

    public override void HandleStaminaDepleted()
    {
        if (IsFailing)
            return;

        IsFailing = true;
        _isBreaking = false;

        StartCoroutine(RageFailure());
    }

    private IEnumerator RageFailure()
    {
        float timer = 0f;
        float direction = 1f;
        float switchTimer = 0f;

        UpdateAnimatorParams(rageAnimator, 0f, Rb.linearVelocity.y, IsGrounded());
        if (CanUseAnimator(rageAnimator))
            rageAnimator.ResetTrigger("Break");

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

            Rb.linearVelocity = new Vector2(direction * failureMoveSpeed * 3f, Rb.linearVelocity.y);

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

        Rb.linearVelocity = Vector2.zero;
        transform.localScale = originalScale;

        GameEvents.RaiseGameOver();
    }

    private void PlayBreakAnimation()
    {
        if (!CanUseAnimator(rageAnimator))
            return;

        StopCoroutine(nameof(BreakAnimationLock));
        StartCoroutine(nameof(BreakAnimationLock));

        rageAnimator.ResetTrigger("Break");
        rageAnimator.SetTrigger("Break");
    }

    private IEnumerator BreakAnimationLock()
    {
        _isBreaking = true;

        yield return new WaitForSeconds(breakAnimationLockTime);

        _isBreaking = false;
    }
}
