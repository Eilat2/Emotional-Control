using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    public enum JoyHitReaction
    {
        Knockback, // קפיצה אחורה
        SlamDown   // בום למטה
    }

    [Header("Stamina Drain")]
    [SerializeField] float drainAmount = 15f;

    [Header("Hit Timing")]
    [SerializeField] float hitCooldown = 0.05f;

    [Header("Hurt Lock + i-frames")]
    [SerializeField] float hurtLockTime = 0.18f;
    [SerializeField] float invincibleTime = 0.4f;

    [Header("Rage Knockback (strong)")]
    [SerializeField] float rageKnockbackX = 10f;
    [SerializeField] float rageKnockbackY = 5f;

    [Header("Joy Reaction")]
    [SerializeField] JoyHitReaction joyReaction = JoyHitReaction.Knockback;

    [Header("Joy Knockback (for Armored)")]
    [SerializeField] float joyKnockbackX = 10f;
    [SerializeField] float joyKnockbackY = 6f;

    [Header("Joy Slam Down (for Flying)")]
    [SerializeField] float joySlamDownY = 12f;
    [SerializeField] float joyPushBackX = 0f;

    [Header("Ignore Stomp From Above (optional)")]
    [SerializeField] bool ignoreIfPlayerStompsFromAbove = false; // למשוריין: עדיף false
    [SerializeField] float stompTolerance = 0.02f;

    float nextHitTime = 0f;
    Collider2D myCol;

    void Awake()
    {
        myCol = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision) => TryHit(collision);
    void OnCollisionStay2D(Collision2D collision) => TryHit(collision);

    void TryHit(Collision2D collision)
    {
        if (Time.time < nextHitTime) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (myCol == null) return;

        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        EmotionController emotion = collision.gameObject.GetComponent<EmotionController>();
        if (emotion == null) return;

        // i-frames
        PlayerHurtLock hurt = collision.gameObject.GetComponent<PlayerHurtLock>();
        if (hurt != null && hurt.IsInvincible) return;

        // סטומפ (בעיקר למעופף) – למשוריין בדרך כלל false
        if (ignoreIfPlayerStompsFromAbove)
        {
            float playerBottomY = collision.collider.bounds.min.y;
            float enemyTopY = myCol.bounds.max.y;

            bool cameFromAbove = playerBottomY >= enemyTopY - stompTolerance;
            bool fallingOrNotRising = rb.linearVelocity.y <= 0.5f;

            if (cameFromAbove && fallingOrNotRising)
                return;
        }

        nextHitTime = Time.time + hitCooldown;

        // מורידים סטאמינה לפי הרגש הנוכחי
        DrainStaminaByCurrentEmotion(collision.collider, emotion);

        // HurtLock + i-frames
        if (hurt != null)
            hurt.TriggerHit(hurtLockTime, invincibleTime);

        // תגובת פגיעה
        if (emotion.current == EmotionController.Emotion.Rage)
        {
            ApplyKnockback(rb, collision.transform, rageKnockbackX, rageKnockbackY);
        }
        else if (emotion.current == EmotionController.Emotion.Joy)
        {
            if (joyReaction == JoyHitReaction.Knockback)
                ApplyKnockback(rb, collision.transform, joyKnockbackX, joyKnockbackY);
            else
                ApplyJoySlamDown(rb, collision.transform);
        }
        else
        {
            // Neutral – אפשר להחליט אחרת
            ApplyKnockback(rb, collision.transform, 8f, 4f);
        }
    }

    void DrainStaminaByCurrentEmotion(Collider2D playerCol, EmotionController emotion)
    {
        if (emotion.current == EmotionController.Emotion.Neutral) return;

        Stamina.StaminaType typeToDrain =
            (emotion.current == EmotionController.Emotion.Joy)
            ? Stamina.StaminaType.Joy
            : Stamina.StaminaType.Rage;

        Stamina[] staminas = playerCol.GetComponents<Stamina>();
        for (int i = 0; i < staminas.Length; i++)
        {
            if (staminas[i].type == typeToDrain)
            {
                staminas[i].Use(drainAmount);
                break;
            }
        }
    }

    // קפיצה אחורה מורגשת
    void ApplyKnockback(Rigidbody2D rb, Transform playerTf, float x, float y)
    {
        float dir = (playerTf.position.x < transform.position.x) ? -1f : 1f;

        // כופים מהירות מורגשת (לא "מקס" קטן)
        rb.linearVelocity = new Vector2(dir * x, y);
    }

    // בום למטה
    void ApplyJoySlamDown(Rigidbody2D rb, Transform playerTf)
    {
        float dir = (playerTf.position.x < transform.position.x) ? -1f : 1f;

        rb.linearVelocity = new Vector2(dir * joyPushBackX, -joySlamDownY);
    }
}
