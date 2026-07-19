using System.Collections;
using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    public enum JoyHitReaction
    {
        Knockback,
        SlamDown
    }

    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Hit Timing")]
    [SerializeField] private float hitCooldown = 0.05f;

    [Header("Hurt Lock + i-frames")]
    [SerializeField] private float hurtLockTime = 0.18f;
    [SerializeField] private float invincibleTime = 0.4f;

    [Header("Neutral Game Over Delay")]
    [SerializeField] private float neutralGameOverDelay = 0.9f;

    [Header("Rage Knockback")]
    [SerializeField] private float rageKnockbackX = 10f;
    [SerializeField] private float rageKnockbackY = 5f;

    [Header("Joy Reaction")]
    [SerializeField] private JoyHitReaction joyReaction = JoyHitReaction.Knockback;

    [Header("Joy Knockback")]
    [SerializeField] private float joyKnockbackX = 10f;
    [SerializeField] private float joyKnockbackY = 6f;

    [Header("Joy Slam Down")]
    [SerializeField] private float joySlamDownY = 12f;
    [SerializeField] private float joyPushBackX = 0f;

    [Header("Ignore Stomp From Above")]
    [SerializeField] private bool ignoreIfPlayerStompsFromAbove = false;
    [SerializeField] private float stompTolerance = 0.5f;

    private float _nextHitTime;
    private Collider2D _myCollider;

    private void Awake()
    {
        _myCollider = GetComponent<Collider2D>();

        if (enemyStats == null)
            Debug.LogError($"{gameObject.name}: EnemyStats is not assigned on EnemyTouchDamage!");
    }

    private void OnCollisionEnter2D(Collision2D collision) => TryHit(collision);
    private void OnCollisionStay2D(Collision2D collision) => TryHit(collision);

    private void TryHit(Collision2D collision)
    {
        if (Time.time < _nextHitTime) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (_myCollider == null) return;

        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        EmotionController emotion = collision.gameObject.GetComponent<EmotionController>();
        if (emotion == null) return;

        PlayerHurtLock hurt = collision.gameObject.GetComponent<PlayerHurtLock>();
        if (hurt != null && hurt.IsInvincible) return;

        if (ignoreIfPlayerStompsFromAbove && IsBeingStompedFromAbove(collision, rb))
            return;

        _nextHitTime = Time.time + hitCooldown;

        PlayerHitFeedback feedback = collision.gameObject.GetComponent<PlayerHitFeedback>();

        if (emotion.current == EmotionController.Emotion.Neutral)
        {
            hurt?.TriggerHit(hurtLockTime, invincibleTime);
            feedback?.PlayHitFeedback();
            ApplyKnockback(rb, collision.transform, 8f, 4f);
            StartCoroutine(NeutralGameOverAfterDelay(collision.gameObject));
            return;
        }

        DrainStaminaByCurrentEmotion(collision.collider, emotion);

        hurt?.TriggerHit(hurtLockTime, invincibleTime);
        feedback?.PlayHitFeedback();

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

    private bool IsBeingStompedFromAbove(Collision2D collision, Rigidbody2D playerRb)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -stompTolerance && playerRb.linearVelocity.y <= 0f)
                return true;
        }

        return false;
    }

    private IEnumerator NeutralGameOverAfterDelay(GameObject player)
    {
        yield return new WaitForSeconds(neutralGameOverDelay);

        PlayerEmotionContext context = player.GetComponent<PlayerEmotionContext>();
        context?.OnStaminaDepleted();
    }

    private void DrainStaminaByCurrentEmotion(Collider2D playerCol, EmotionController emotion)
    {
        float drainAmount = enemyStats != null ? enemyStats.staminaDrain : 0f;

        Stamina.StaminaType typeToDrain =
            emotion.current == EmotionController.Emotion.Joy
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

    private void ApplyKnockback(Rigidbody2D rb, Transform playerTf, float x, float y)
    {
        float dir = playerTf.position.x < transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * x, y);
    }

    private void ApplyJoySlamDown(Rigidbody2D rb, Transform playerTf)
    {
        float dir = playerTf.position.x < transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * joyPushBackX, -joySlamDownY);
    }
}
