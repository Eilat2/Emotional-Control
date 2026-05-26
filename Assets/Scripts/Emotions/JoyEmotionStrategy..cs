using UnityEngine;
using System.Collections;

public class JoyEmotionStrategy : MonoBehaviour, IEmotionStrategy
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

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

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

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;
    private Collider2D playerCollider;
    private PlayerStateMachine stateMachine;

    private Vector2 moveInput;

    private bool jumpedFromGround = false;
    private bool glideEnabled = false;
    private bool jumpHeld = false;

    private Coroutine jumpTrailCoroutine;

    private bool isFailing = false;
    //TODO:
    //bool grounded = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
        playerCollider = GetComponent<Collider2D>();
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    void Start()
    {
        rb.gravityScale = normalGravity;
        ResolveJoyStamina();
        ResolveJoyAnimator();
    }

    public void Enter()
    {
        StopAllCoroutines();
        ResetJoyState();
    }

    public void Exit()
    {
        if (isFailing)
            return;

        StopAllCoroutines();
        ResetJoyState();
    }

    private void ResetJoyState()
    {
        isFailing = false;
        glideEnabled = false;
        jumpHeld = false;
        jumpedFromGround = false;
        moveInput = Vector2.zero;
        jumpTrailCoroutine = null;

        if (stateMachine != null)
            stateMachine.StopGlide();

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = normalGravity;
        }

        if (joyAnimator != null && joyAnimator.runtimeAnimatorController != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", false);
            joyAnimator.SetFloat("yVelocity", 0f);
            joyAnimator.SetBool("isGrounded", true);
        }

        if (glideTrail != null && glideTrail.isPlaying)
            glideTrail.Stop();
    }

    public void HandleMove(Vector2 move)
    {
        if (isFailing) return;

        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        if (isFailing) return;

        jumpHeld = isHeld;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        bool grounded = IsGrounded();

        if (grounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpedFromGround = false;

            if (glideEnabled && stateMachine != null)
                stateMachine.StopGlide();

            glideEnabled = false;
            rb.gravityScale = normalGravity;

            if (glideTrail != null && glideTrail.isPlaying)
                glideTrail.Stop();
        }

        if (glideEnabled && releasedThisFrame)
        {
            glideEnabled = false;

            if (stateMachine != null)
                stateMachine.StopGlide();

            rb.gravityScale = normalGravity;

            if (glideTrail != null && glideTrail.isPlaying)
                glideTrail.Stop();

            return;
        }

        if (pressedThisFrame && grounded)
        {
            jumpedFromGround = true;
            glideEnabled = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * joyJumpForce, ForceMode2D.Impulse);
            //TODO:
            //grounded = false;
            if (jumpTrailCoroutine != null)
                StopCoroutine(jumpTrailCoroutine);

            jumpTrailCoroutine = StartCoroutine(PlayJumpTrail());

            return;
        }

        if (pressedThisFrame && !grounded && jumpedFromGround && !glideEnabled)
        {
            if (joyStamina != null && joyStamina.currentStamina <= 0f)
            {
                HandleStaminaDepleted();
                return;
            }

            glideEnabled = true;

            if (stateMachine != null)
                stateMachine.StartGlide();

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);

            if (glideTrail != null && !glideTrail.isPlaying)
                glideTrail.Play();
        }
    }

    public void Tick()
    {
        if (isFailing) return;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        bool grounded = IsGrounded();

        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", Mathf.Abs(moveInput.x));
            joyAnimator.SetBool("isGliding", glideEnabled);

            // פרמטרים בשביל Fall Animation
            joyAnimator.SetFloat("yVelocity", rb.linearVelocity.y);
            joyAnimator.SetBool("isGrounded", grounded);
        }

        if (!glideEnabled)
        {
            rb.gravityScale = normalGravity;

            if (jumpTrailCoroutine == null && glideTrail != null && glideTrail.isPlaying)
                glideTrail.Stop();

            return;
        }

        if (jumpHeld)
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

            rb.gravityScale = glideGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    public void HandleStaminaDepleted()
    {
        if (isFailing) return;

        isFailing = true;

        if (stateMachine != null)
            stateMachine.StopGlide();

        StartCoroutine(JoyFailure());
    }

    private IEnumerator JoyFailure()
    {
        glideEnabled = false;
        jumpHeld = false;
        moveInput = Vector2.zero;

        if (glideTrail != null && glideTrail.isPlaying)
            glideTrail.Stop();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", true);
            joyAnimator.SetFloat("yVelocity", 0f);
            joyAnimator.SetBool("isGrounded", false);
        }

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

        if (!glideEnabled && glideTrail != null && glideTrail.isPlaying)
            glideTrail.Stop();

        jumpTrailCoroutine = null;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void ResolveJoyStamina()
    {
        if (joyStamina != null) return;

        Stamina[] all = GetComponentsInChildren<Stamina>();

        foreach (var s in all)
        {
            if (s.type == Stamina.StaminaType.Joy)
            {
                joyStamina = s;
                return;
            }
        }
    }

    private void ResolveJoyAnimator()
    {
        if (joyAnimator != null) return;

        Transform joyVisual = transform.Find("JoyVisual");

        if (joyVisual != null)
            joyAnimator = joyVisual.GetComponent<Animator>();
    }

    //TODO: New ground check
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.layer == groundLayer )
    //    {
    //        grounded = true;
    //    }
    //}
}