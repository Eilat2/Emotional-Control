using System.Collections;
using UnityEngine;

public class FlyingEnemyStomp : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Bounce")]
    [SerializeField] private float stompBounceForce = 8f;

    [Header("Stomp Check")]
    [SerializeField] private float stompHeightOffset = 0.25f;

    [Header("Touch Damage Cooldown After Stomp")]
    // כמה זמן EnemyTouchDamage נשאר כבוי אחרי דריכה, כדי שלא ייספר
    // גם כפגיעת מגע וגם כדריכה באותו רגע. בעבר זה כובה לצמיתות בטעות
    // ולא הודלק בחזרה - עכשיו זה חוזר לפעול אחרי הזמן הזה.
    [SerializeField] private float touchDamageDisableDuration = 0.3f;

    private Collider2D _enemyCollider;
    private EnemyTouchDamage _enemyDamage;
    private EnemyHealthSystem _enemyHealth;

    private Coroutine _reEnableRoutine;

    private void Awake()
    {
        _enemyCollider = GetComponent<Collider2D>();
        _enemyDamage = GetComponent<EnemyTouchDamage>();
        _enemyHealth = GetComponentInParent<EnemyHealthSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyStats == null || !enemyStats.canBeStompedByJoy)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        EmotionController emotion = collision.gameObject.GetComponent<EmotionController>();
        if (emotion == null || emotion.current != EmotionController.Emotion.Joy)
            return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb == null || _enemyCollider == null)
            return;

        if (!IsPlayerStompingFromAbove(collision, playerRb))
            return;

        TemporarilyDisableTouchDamage();

        PlayerHurtLock hurt = collision.gameObject.GetComponent<PlayerHurtLock>();
        hurt?.TriggerHit(0f, 0.2f);

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);

        if (_enemyHealth != null)
            _enemyHealth.TakeDamage(enemyStats.stompDamage);
        else
            Debug.LogError($"{gameObject.name}: EnemyHealthSystem not found!");
    }

    private void TemporarilyDisableTouchDamage()
    {
        if (_enemyDamage == null)
            return;

        _enemyDamage.enabled = false;

        // אם דרכו עליו שוב לפני שהקווייה הקודמת סיימה (אויב עם כמה חיים) -
        // מפעילים מחדש את הטיימר, לא צוברים כמה קורוטינות במקביל.
        if (_reEnableRoutine != null)
            StopCoroutine(_reEnableRoutine);

        _reEnableRoutine = StartCoroutine(ReEnableTouchDamageAfterDelay());
    }

    private IEnumerator ReEnableTouchDamageAfterDelay()
    {
        yield return new WaitForSeconds(touchDamageDisableDuration);

        if (_enemyDamage != null)
            _enemyDamage.enabled = true;

        _reEnableRoutine = null;
    }

    private bool IsPlayerStompingFromAbove(Collision2D collision, Rigidbody2D playerRb)
    {
        if (playerRb.linearVelocity.y > 0f)
            return false;

        Collider2D playerCol = collision.collider;
        if (playerCol == null)
            return false;

        float playerBottomY = playerCol.bounds.min.y;
        float enemyTopY = _enemyCollider.bounds.max.y;

        return playerBottomY >= enemyTopY - stompHeightOffset;
    }
}
