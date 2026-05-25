using UnityEngine;

// אויב שניתן להרוג ע"י Rage
public class KillableEnemy : MonoBehaviour, IBreakable
{
    [Header("Debris")]
    [SerializeField] private GameObject debrisPrefab; // פריפאב של חתיכה
    [SerializeField] private int debrisCount = 5;     // כמה חתיכות ליצור
    [SerializeField] private float force = 4f;        // עוצמת העפה
    [SerializeField] private float torque = 4f;       // סיבוב לחתיכות

    private bool isDead = false; // מונע ספירה כפולה אם האויב נפגע פעמיים מהר

    // פונקציה שנקראת כשהשחקן פוגע בו
    public void OnBreak()
    {
        Die();
    }

    // מוות של האויב
    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        SpawnDebris();

        // אם בסצנה קיים EnemyLevelCounter,
        // נעדכן אותו שאויב אחד מת.
        // אם אין כזה בסצנה - לא יקרה כלום.
        EnemyLevelCounter counter = FindObjectOfType<EnemyLevelCounter>();

        if (counter != null)
        {
            counter.EnemyDied();
        }

        Destroy(gameObject);
    }

    private void SpawnDebris()
    {
        if (debrisPrefab == null)
            return;

        for (int i = 0; i < debrisCount; i++)
        {
            GameObject piece = Instantiate(
                debrisPrefab,
                transform.position,
                Quaternion.identity
            );

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 dir = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f)
                ).normalized;

                rb.AddForce(dir * force, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
            }
        }
    }
}