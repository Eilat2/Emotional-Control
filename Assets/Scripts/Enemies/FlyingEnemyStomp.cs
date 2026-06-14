using UnityEngine;

public class FlyingEnemyStomp : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Bounce")]
    [SerializeField] private float stompBounceForce = 8f;

    [Header("Stomp Check")]
    [SerializeField] private float stompHeightOffset = 0.25f;

    private Collider2D enemyCol;
    private EnemyTouchDamage enemyDamage;
    private EnemyHealthSystem enemyHealth;

    private void Awake()
    {
        enemyCol = GetComponent<Collider2D>();
        enemyDamage = GetComponent<EnemyTouchDamage>();
        enemyHealth = GetComponentInParent<EnemyHealthSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyStats == null)
            return;

        if (!enemyStats.canBeStompedByJoy)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        EmotionController emotion = collision.gameObject.GetComponent<EmotionController>();
        if (emotion == null || emotion.current != EmotionController.Emotion.Joy)
            return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb == null)
            return;

        if (enemyCol == null)
            return;

        if (!IsPlayerStompingFromAbove(collision, playerRb))
            return;

        if (enemyDamage != null)
            enemyDamage.enabled = false;

        PlayerHurtLock hurt = collision.gameObject.GetComponent<PlayerHurtLock>();
        if (hurt != null)
            hurt.TriggerHit(0f, 0.2f);

        playerRb.linearVelocity = new Vector2(
            playerRb.linearVelocity.x,
            stompBounceForce
        );

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(enemyStats.stompDamage);
        }
        else
        {
            Debug.LogError($"{gameObject.name}: EnemyHealthSystem not found!");
        }
    }

    private bool IsPlayerStompingFromAbove(Collision2D collision, Rigidbody2D playerRb)
    {
        if (playerRb.linearVelocity.y > 0f)
            return false;

        Collider2D playerCol = collision.collider;
        if (playerCol == null)
            return false;

        float playerBottomY = playerCol.bounds.min.y;
        float enemyTopY = enemyCol.bounds.max.y;

        return playerBottomY >= enemyTopY - stompHeightOffset;
    }
}