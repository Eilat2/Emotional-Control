using UnityEngine;

/// <summary>
/// דריכה על אויב מעופף - עובדת רק כששמחה פעילה (Joy).
/// אם השחקן "מעל" האויב בעת ההתנגשות:
/// 1) השחקן מקבל קפיצה קטנה למעלה
/// 2) האויב מת (דרך KillableEnemy אם קיים, אחרת Destroy)
/// </summary>
public class FlyingEnemyStomp : MonoBehaviour
{
    [Header("Bounce")]
    [SerializeField] private float stompBounceForce = 8f;
    // כמה חזק השחקן יקפוץ אחרי דריכה

    [Header("Stomp Check")]
    [SerializeField] private float stompHeightOffset = 0.3f;
    // מרווח סלחני: כמה מספיק שהשחקן יהיה "מעל" האויב כדי להיחשב דריכה
    // (מומלץ 0.2–0.4 במיוחד אם יש ריחוף/גלייד)

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

        // חייב Collider לאויב כדי לדעת איפה "הראש" שלו
        Collider2D enemyCol = GetComponent<Collider2D>();
        if (enemyCol == null)
            return;

        // תחתית השחקן מול ראש האויב (בדיקת "מעל")
        float playerBottomY = collision.collider.bounds.min.y;
        float enemyTopY = enemyCol.bounds.max.y;

        bool isAbove = playerBottomY >= enemyTopY - stompHeightOffset;
        if (!isAbove)
            return;

        // קפיצה לשחקן אחרי דריכה
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);

        // הריגת האויב (אם יש קומפוננטה שמטפלת במוות – נשתמש בה)
        KillableEnemy killable = GetComponent<KillableEnemy>();
        if (killable != null)
            killable.OnBreak();
        else
            Destroy(gameObject);
    }
}
