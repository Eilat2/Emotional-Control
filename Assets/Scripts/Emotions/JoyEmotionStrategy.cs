using UnityEngine;
using System.Collections;

public class JoyEmotionStrategy : EmotionStrategyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float joyJumpForce = 6f;

    [Header("Glide (2nd press)")]
    [SerializeField] private float normalGravity = 4f;
    [SerializeField] private float glideGravity = 1.2f;
    [SerializeField] private float floatUpImpulse = 8f;

    [Header("Stamina (Joy)")]
    [SerializeField] private float glideCostPerSecond = 15f;
    [SerializeField] private Stamina joyStamina;

    [Header("Animation")]
    [SerializeField] private Animator joyAnimator;

    [Header("Glide Trail")]
    [SerializeField] private ParticleSystem glideTrail;
    [SerializeField] private float jumpTrailDuration = 0.25f;

    [Header("Joy Failure")]
    [SerializeField] private float failureDuration = 3.5f;
    [SerializeField] private float failureUpSpeed = 9f;
    [SerializeField] private float failureSideAmount = 2.5f;
    [SerializeField] private float failureSideSpeed = 3.5f;

    private Collider2D _playerCollider;

    private bool _jumpedFromGround;
    private bool _glideEnabled;
    private bool _jumpHeld;

    private Coroutine _jumpTrailCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _playerCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Rb.gravityScale = normalGravity;

        if (joyStamina == null)
            joyStamina = FindStamina(Stamina.StaminaType.Joy, includeChildren: true);

        joyAnimator = ResolveAnimator(joyAnimator, "JoyVisual");
    }

    public override void Enter()
    {
        StopAllCoroutines();
        ResetJoyState();
    }

    public override void Exit()
    {
        if (IsFailing)
            return;

        StopAllCoroutines();
        ResetJoyState();
    }

    private void ResetJoyState()
    {
        IsFailing = false;
        _glideEnabled = false;
        _jumpHeld = false;
        _jumpedFromGround = false;
        MoveInput = Vector2.zero;
        _jumpTrailCoroutine = null;

        StateMachine?.StopGlide();

        if (_playerCollider != null)
            _playerCollider.enabled = true;

        if (Rb != null)
        {
            Rb.simulated = true;
            Rb.linearVelocity = Vector2.zero;
            Rb.angularVelocity = 0f;
            Rb.gravityScale = normalGravity;
        }

        UpdateAnimatorParams(joyAnimator, 0f, 0f, true);
        if (CanUseAnimator(joyAnimator))
            joyAnimator.SetBool("isGliding", false);

        StopGlideTrail();
    }

    public override void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        if (IsFailing)
            return;

        _jumpHeld = isHeld;

        if (HurtLock != null && HurtLock.IsLocked)
            return;

        bool grounded = IsGrounded();

        HandleLandingReset(grounded);

        if (_glideEnabled && releasedThisFrame)
        {
            StopGliding();
            return;
        }

        if (pressedThisFrame && grounded)
        {
            PerformGroundJump();
            return;
        }

        if (pressedThisFrame && !grounded && _jumpedFromGround && !_glideEnabled)
        {
            TryStartGlide();
        }
    }

    private void HandleLandingReset(bool grounded)
    {
        if (!(grounded && Rb.linearVelocity.y <= 0.01f))
            return;

        _jumpedFromGround = false;

        if (_glideEnabled && StateMachine != null)
            StateMachine.StopGlide();

        _glideEnabled = false;
        Rb.gravityScale = normalGravity;
        StopGlideTrail();
    }

    private void StopGliding()
    {
        _glideEnabled = false;

        if (StateMachine != null)
            StateMachine.StopGlide();

        Rb.gravityScale = normalGravity;
        StopGlideTrail();
    }

    private void PerformGroundJump()
    {
        _jumpedFromGround = true;
        _glideEnabled = false;

        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, 0f);
        Rb.AddForce(Vector2.up * joyJumpForce, ForceMode2D.Impulse);

        if (_jumpTrailCoroutine != null)
            StopCoroutine(_jumpTrailCoroutine);

        _jumpTrailCoroutine = StartCoroutine(PlayJumpTrail());
    }

    private void TryStartGlide()
    {
        if (joyStamina != null && joyStamina.currentStamina <= 0f)
        {
            HandleStaminaDepleted();
            return;
        }

        _glideEnabled = true;

        if (StateMachine != null)
            StateMachine.StartGlide();

        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, 0f);
        Rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);

        if (glideTrail != null && !glideTrail.isPlaying)
            glideTrail.Play();
    }

    public override void Tick()
    {
        if (IsFailing)
            return;

        if (HurtLock != null && HurtLock.IsLocked)
            return;

        Rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, Rb.linearVelocity.y);

        bool grounded = IsGrounded();

        UpdateAnimatorParams(joyAnimator, Mathf.Abs(MoveInput.x), Rb.linearVelocity.y, grounded);
        if (CanUseAnimator(joyAnimator))
            joyAnimator.SetBool("isGliding", _glideEnabled);

        if (!_glideEnabled)
        {
            Rb.gravityScale = normalGravity;

            if (_jumpTrailCoroutine == null)
                StopGlideTrail();

            return;
        }

        if (_jumpHeld)
        {
            float cost = glideCostPerSecond * Time.deltaTime;

            if (joyStamina != null)
            {
                bool hasStamina = joyStamina.Use(cost);

                if (!hasStamina)
                {
                    HandleStaminaDepleted();
                    return;
                }
            }

            Rb.gravityScale = glideGravity;
        }
        else
        {
            Rb.gravityScale = normalGravity;
        }
    }

    public override void HandleStaminaDepleted()
    {
        if (IsFailing)
            return;

        IsFailing = true;

        StateMachine?.StopGlide();

        StartCoroutine(JoyFailure());
    }

    private IEnumerator JoyFailure()
    {
        _glideEnabled = false;
        _jumpHeld = false;
        MoveInput = Vector2.zero;

        StopGlideTrail();

        Rb.linearVelocity = Vector2.zero;
        Rb.angularVelocity = 0f;
        Rb.gravityScale = 0f;

        if (_playerCollider != null)
            _playerCollider.enabled = true;

        UpdateAnimatorParams(joyAnimator, 0f, 0f, false);
        if (CanUseAnimator(joyAnimator))
            joyAnimator.SetBool("isGliding", true);

        float timer = 0f;
        Vector3 startPosition = transform.position;

        while (timer < failureDuration)
        {
            timer += Time.deltaTime;

            float upMovement = timer * failureUpSpeed;
            float sideMovement = Mathf.Sin(timer * failureSideSpeed) * failureSideAmount;

            transform.position = startPosition + new Vector3(sideMovement, upMovement, 0f);

            yield return null;
        }

        GameEvents.RaiseGameOver();
    }

    private IEnumerator PlayJumpTrail()
    {
        if (glideTrail != null && !glideTrail.isPlaying)
            glideTrail.Play();

        yield return new WaitForSeconds(jumpTrailDuration);

        if (!_glideEnabled)
            StopGlideTrail();

        _jumpTrailCoroutine = null;
    }

    private void StopGlideTrail()
    {
        if (glideTrail != null && glideTrail.isPlaying)
            glideTrail.Stop();
    }
}
