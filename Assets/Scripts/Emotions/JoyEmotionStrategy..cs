using UnityEngine;
using System.Collections;

public class JoyEmotionStrategy : MonoBehaviour, IEmotionStrategy
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float joyJumpForce = 6f;

    [Header("Glide (2nd press)")]
    [SerializeField] private float normalGravity = 4f;      // כבידה רגילה
    [SerializeField] private float glideGravity = 1.2f;     // כבידה בזמן ריחוף
    [SerializeField] private float floatUpImpulse = 8f;     // דחיפה למעלה בתחילת ריחוף

    [Header("Stamina (Joy)")]
    [SerializeField] private float glideCostPerSecond = 15f; // כמה סטאמינה יורדת לשנייה בזמן ריחוף
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
    [SerializeField] private float failureDuration = 3.5f;      // זמן הפסילה
    [SerializeField] private float failureUpSpeed = 9f;         // מהירות עלייה
    [SerializeField] private float failureSideAmount = 2.5f;    // מרחק תנועה ימינה ושמאלה
    [SerializeField] private float failureSideSpeed = 3.5f;     // מהירות התנועה לצדדים

    private Rigidbody2D rb;
    private PlayerHurtLock hurtLock;
    private Collider2D playerCollider;

    private Vector2 moveInput;

    private bool jumpedFromGround = false;
    private bool glideEnabled = false;
    private bool jumpHeld = false;

    private Coroutine jumpTrailCoroutine;

    private bool isFailing = false; // האם Joy כרגע באמצע פסילה

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtLock = GetComponent<PlayerHurtLock>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        rb.gravityScale = normalGravity;
        ResolveJoyStamina();
        ResolveJoyAnimator();
    }

    public void Enter()
    {
        // כשנכנסים ל-Joy מתחילים ממצב נקי
        StopAllCoroutines();
        ResetJoyState();
    }

    public void Exit()
    {
        // חשוב:
        // אם Joy באמצע פסילה, אסור לעצור את ה-Coroutine.
        // אחרת היא תעוף החוצה אבל לא תגיע לשלב של פתיחת Game Over.
        if (isFailing)
            return;

        StopAllCoroutines();
        ResetJoyState();
    }

    private void ResetJoyState()
    {
        // מאפסים דגלים פנימיים
        isFailing = false;
        glideEnabled = false;
        jumpHeld = false;
        jumpedFromGround = false;
        moveInput = Vector2.zero;
        jumpTrailCoroutine = null;

        // מחזירים קוליידר ליתר ביטחון
        if (playerCollider != null)
            playerCollider.enabled = true;

        // מחזירים פיזיקה תקינה
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = normalGravity;
        }

        // מאפסים אנימציה
        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", false);
        }

        // מכבים שובל ריחוף
        if (glideTrail != null && glideTrail.isPlaying)
        {
            glideTrail.Stop();
        }
    }

    public void HandleMove(Vector2 move)
    {
        // בזמן פסילה אין שליטה בתנועה
        if (isFailing) return;

        moveInput = move;
    }

    public void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame)
    {
        // בזמן פסילה לא מאפשרים קפיצה/ריחוף
        if (isFailing) return;

        jumpHeld = isHeld;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        bool grounded = IsGrounded();

        // אם נחתנו – מאפסים מצב ריחוף
        if (grounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpedFromGround = false;
            glideEnabled = false;
            rb.gravityScale = normalGravity;

            // מכבים שובל ריחוף
            if (glideTrail != null && glideTrail.isPlaying)
            {
                glideTrail.Stop();
            }
        }

        // אם שחררנו את הכפתור בזמן ריחוף – מפסיקים לרחף
        if (glideEnabled && releasedThisFrame)
        {
            glideEnabled = false;
            rb.gravityScale = normalGravity;

            // מכבים שובל ריחוף
            if (glideTrail != null && glideTrail.isPlaying)
            {
                glideTrail.Stop();
            }

            return;
        }

        // קפיצה רגילה מהקרקע
        if (pressedThisFrame && grounded)
        {
            jumpedFromGround = true;
            glideEnabled = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * joyJumpForce, ForceMode2D.Impulse);

            // מפעילים שובל גם בקפיצה רגילה לזמן קצר
            if (jumpTrailCoroutine != null)
                StopCoroutine(jumpTrailCoroutine);

            jumpTrailCoroutine = StartCoroutine(PlayJumpTrail());

            return;
        }

        // התחלת ריחוף באוויר בלחיצה שנייה
        if (pressedThisFrame && !grounded && jumpedFromGround && !glideEnabled)
        {
            // אם אין סטאמינה בכלל – נפסלים מיד
            if (joyStamina != null && joyStamina.currentStamina <= 0f)
            {
                HandleStaminaDepleted();
                return;
            }

            glideEnabled = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * floatUpImpulse, ForceMode2D.Impulse);

            // מפעילים שובל ריחוף
            if (glideTrail != null && !glideTrail.isPlaying)
            {
                glideTrail.Play();
            }
        }
    }

    public void Tick()
    {
        // בזמן פסילה לא מריצים תנועה רגילה
        if (isFailing) return;

        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תנועה אופקית רגילה
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // עדכון אנימציות
        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", Mathf.Abs(moveInput.x));
            joyAnimator.SetBool("isGliding", glideEnabled);
        }

        // אם לא מרחפים – כבידה רגילה
        if (!glideEnabled)
        {
            rb.gravityScale = normalGravity;

            // מכבים שובל אם לא מרחפים וגם לא באמצע שובל של קפיצה רגילה
            if (jumpTrailCoroutine == null && glideTrail != null && glideTrail.isPlaying)
            {
                glideTrail.Stop();
            }

            return;
        }

        // אם מרחפים ומחזיקים כפתור – צורכים סטאמינה
        if (jumpHeld)
        {
            float cost = glideCostPerSecond * Time.deltaTime;

            if (joyStamina != null)
            {
                bool hasStamina = joyStamina.Use(cost);

                // ברגע שהסטאמינה מגיעה ל-0 – מפעילים פסילה מיד
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
        // הגנה כדי שהפסילה לא תופעל יותר מפעם אחת
        if (isFailing) return;

        isFailing = true;

        StartCoroutine(JoyFailure());
    }

    private IEnumerator JoyFailure()
    {
        // מבטלים שליטה וריחוף רגיל
        glideEnabled = false;
        jumpHeld = false;
        moveInput = Vector2.zero;

        // מכבים שובל ריחוף
        if (glideTrail != null && glideTrail.isPlaying)
        {
            glideTrail.Stop();
        }

        // מאפסים פיזיקה לפני תעופה
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // מבטלים כבידה כדי שתוכל לעוף למעלה בלי ליפול
        rb.gravityScale = 0f;

        // לא מכבים Collider כדי שלא יהיו נפילות דרך רצפה אחרי Restart
        if (playerCollider != null)
            playerCollider.enabled = true;

        // משאירים אנימציית ריחוף בזמן הפסילה
        if (joyAnimator != null)
        {
            joyAnimator.SetFloat("speed", 0f);
            joyAnimator.SetBool("isGliding", true);
        }

        float timer = 0f;
        Vector3 startPosition = transform.position;

        while (timer < failureDuration)
        {
            timer += Time.deltaTime;

            // עלייה גבוהה למעלה כדי לצאת מהמסך
            float upMovement = timer * failureUpSpeed;

            // תנועה איטית מצד לצד בשביל דרמה
            float sideMovement = Mathf.Sin(timer * failureSideSpeed) * failureSideAmount;

            transform.position = startPosition + new Vector3(sideMovement, upMovement, 0f);

            yield return null;
        }

        // בסוף הפסילה פותחים את מסך ה-Game Over
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

    private IEnumerator PlayJumpTrail()
    {
        if (glideTrail != null && !glideTrail.isPlaying)
        {
            glideTrail.Play();
        }

        yield return new WaitForSeconds(jumpTrailDuration);

        if (!glideEnabled && glideTrail != null && glideTrail.isPlaying)
        {
            glideTrail.Stop();
        }

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
}