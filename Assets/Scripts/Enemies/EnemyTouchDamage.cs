using System.Collections;
using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    public enum JoyHitReaction
    {
        Knockback,
        SlamDown
    }

    [Header("Stamina Drain")]
    [SerializeField] float drainAmount = 15f; // כמה סטאמינה יורדת בפגיעה

    [Header("Hit Timing")]
    [SerializeField] float hitCooldown = 0.05f; // זמן בין פגיעות

    [Header("Hurt Lock + i-frames")]
    [SerializeField] float hurtLockTime = 0.18f;   // זמן שבו השחקן "נעול" אחרי פגיעה
    [SerializeField] float invincibleTime = 0.4f;  // זמן חסינות אחרי פגיעה

    [Header("Neutral Game Over Delay")]
    [SerializeField] float neutralGameOverDelay = 0.9f; // זמן לחכות כדי שיראו את הנוקבק וההבהוב לפני Game Over

    [Header("Rage Knockback")]
    [SerializeField] float rageKnockbackX = 10f;
    [SerializeField] float rageKnockbackY = 5f;

    [Header("Joy Reaction")]
    [SerializeField] JoyHitReaction joyReaction = JoyHitReaction.Knockback;

    [Header("Joy Knockback")]
    [SerializeField] float joyKnockbackX = 10f;
    [SerializeField] float joyKnockbackY = 6f;

    [Header("Joy Slam Down")]
    [SerializeField] float joySlamDownY = 12f;
    [SerializeField] float joyPushBackX = 0f;

    [Header("Ignore Stomp From Above")]
    [SerializeField] bool ignoreIfPlayerStompsFromAbove = false;
    [SerializeField] float stompTolerance = 0.5f;

    private float nextHitTime = 0f;
    private Collider2D myCol;

    private void Awake()
    {
        myCol = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) => TryHit(collision);
    private void OnCollisionStay2D(Collision2D collision) => TryHit(collision);

    private void TryHit(Collision2D collision)
    {
        // מניעת פגיעות רצופות מהר מדי
        if (Time.time < nextHitTime) return;

        // מטפלים רק בשחקן
        if (!collision.gameObject.CompareTag("Player")) return;

        if (myCol == null) return;

        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        // לוקחים את הרגש הנוכחי של השחקן
        EmotionController emotion = collision.gameObject.GetComponent<EmotionController>();
        if (emotion == null) return;

        // בדיקת חסינות אחרי פגיעה קודמת
        PlayerHurtLock hurt = collision.gameObject.GetComponent<PlayerHurtLock>();
        if (hurt != null && hurt.IsInvincible) return;

        // אם זה אויב מעופף, אפשר להתעלם מפגיעה שבה השחקן דרך עליו מלמעלה
        if (ignoreIfPlayerStompsFromAbove)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -stompTolerance && rb.linearVelocity.y <= 0f)
                    return;
            }
        }

        nextHitTime = Time.time + hitCooldown;

        // פידבק ויזואלי כמו הבהוב
        PlayerHitFeedback feedback = collision.gameObject.GetComponent<PlayerHitFeedback>();

        // =======================
        // ניטרלי – קודם מקבל מכה, ורק אחרי דיליי Game Over
        // =======================
        if (emotion.current == EmotionController.Emotion.Neutral)
        {
            // נעילת פגיעה + חסינות קצרה, כדי שהתנועה של המכה תורגש
            if (hurt != null)
                hurt.TriggerHit(hurtLockTime, invincibleTime);

            // הבהוב / אפקט פגיעה
            if (feedback != null)
                feedback.PlayHitFeedback();

            // קפיצה אחורה כדי שיהיה ברור שהאויב פגע בו
            ApplyKnockback(rb, collision.transform, 8f, 4f);

            // מחכים שיראו את כל הפגיעה ואז מפעילים Game Over
            StartCoroutine(NeutralGameOverAfterDelay(collision.gameObject));

            return;
        }

        // =======================
        // Joy / Rage – מורידים סטאמינה לפי הרגש
        // =======================
        DrainStaminaByCurrentEmotion(collision.collider, emotion);

        // נעילת פגיעה + חסינות קצרה
        if (hurt != null)
            hurt.TriggerHit(hurtLockTime, invincibleTime);

        // פידבק חזותי
        if (feedback != null)
            feedback.PlayHitFeedback();

        // תגובת פגיעה לפי רגש
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
    }

    // מחכה לפני Game Over כדי שיראו את הנוקבק וההבהוב של ניטרלי
    private IEnumerator NeutralGameOverAfterDelay(GameObject player)
    {
        yield return new WaitForSeconds(neutralGameOverDelay);

        PlayerEmotionContext context = player.GetComponent<PlayerEmotionContext>();

        if (context != null)
            context.OnStaminaDepleted();
    }

    // מוריד סטאמינה לפי הרגש הנוכחי
    private void DrainStaminaByCurrentEmotion(Collider2D playerCol, EmotionController emotion)
    {
        Stamina.StaminaType typeToDrain =
            (emotion.current == EmotionController.Emotion.Joy)
            ? Stamina.StaminaType.Joy
            : Stamina.StaminaType.Rage;

        Stamina[] staminas = playerCol.GetComponents<Stamina>();

        foreach (Stamina s in staminas)
        {
            if (s.type == typeToDrain)
            {
                s.Use(drainAmount);
                break;
            }
        }
    }

    // קובע לאיזה צד להעיף את השחקן ביחס לאויב
    private void ApplyKnockback(Rigidbody2D rb, Transform playerTf, float x, float y)
    {
        float dir = (playerTf.position.x < transform.position.x) ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * x, y);
    }

    // תגובת Joy מול אויב מעופף: דחיפה הצידה ולמטה
    private void ApplyJoySlamDown(Rigidbody2D rb, Transform playerTf)
    {
        float dir = (playerTf.position.x < transform.position.x) ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * joyPushBackX, -joySlamDownY);
    }
}