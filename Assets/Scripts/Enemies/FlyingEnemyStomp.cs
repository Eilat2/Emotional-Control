using UnityEngine;

/// <summary>
/// דריכה על אויב מעופף - עובדת רק כששמחה פעילה (Joy).
/// אם השחקן פוגע באויב מלמעלה:
/// 1) השחקן מקבל קפיצה למעלה
/// 2) האויב מת
/// אם הפגיעה היא מהצד או מלמטה - האויב לא מת,
/// ואז EnemyTouchDamage יפגע בשחקן.
/// </summary>
public class FlyingEnemyStomp : MonoBehaviour
{
    [Header("Bounce")]
    [SerializeField] private float stompBounceForce = 8f; // עוצמת הקפיצה של השחקן אחרי דריכה

    [Header("Stomp Check")]
    [SerializeField] private float stompHeightOffset = 0.25f; // כמה סלחנות יש לדריכה מלמעלה

    private Collider2D enemyCol;
    private EnemyTouchDamage enemyDamage;

    private void Awake()
    {
        enemyCol = GetComponent<Collider2D>();
        enemyDamage = GetComponent<EnemyTouchDamage>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // חייב להיות שחקן
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // חייב להיות Joy
        EmotionController emotion = collision.gameObject.GetComponent<EmotionController>();
        if (emotion == null || emotion.current != EmotionController.Emotion.Joy)
            return;

        // חייב Rigidbody לשחקן כדי לתת קפיצה
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb == null)
            return;

        // חייב Collider לאויב
        if (enemyCol == null)
            return;

        // אם זו לא דריכה מלמעלה - לא הורגים
        if (!IsPlayerStompingFromAbove(collision, playerRb))
            return;

        // מכבים נזק לפני כל דבר אחר
        if (enemyDamage != null)
            enemyDamage.enabled = false;

        // נותנים לשחקנית i-frames רגעיים אם יש לה HurtLock
        PlayerHurtLock hurt = collision.gameObject.GetComponent<PlayerHurtLock>();
        if (hurt != null)
            hurt.TriggerHit(0f, 0.2f);

        // קפיצה קטנה למעלה אחרי הדריכה
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);

        // הורגים את האויב
        KillableEnemy killable = GetComponent<KillableEnemy>();
        if (killable != null)
            killable.OnBreak();
        else
            Destroy(gameObject);
    }

    private bool IsPlayerStompingFromAbove(Collision2D collision, Rigidbody2D playerRb)
    {
        // חייבת להיות בתנועה כלפי מטה או לפחות לא בעלייה
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