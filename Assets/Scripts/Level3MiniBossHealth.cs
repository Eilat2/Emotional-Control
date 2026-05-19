using UnityEngine;

// מיני בוס של שלב 3
// דורש כמה פגיעות לפני שהוא מת
public class Level3MiniBossHealth : MonoBehaviour, IBreakable
{
    [Header("כמה פגיעות צריך כדי להרוג את הבוס")]
    [SerializeField] private int hitsToKill = 3;

    [Header("תצוגת חיים מעל הבוס")]
    [SerializeField] private GameObject[] hpVisuals;

    [Header("Joy Attack")]
    [SerializeField] private float stompBounceForce = 6f;

    [Header("Debris")]
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int debrisCount = 8;
    [SerializeField] private float force = 5f;
    [SerializeField] private float torque = 5f;

    [Header("המעלית של שלב 3")]
    [SerializeField] private Level3SecretElevator elevator;

    [Header("הדלת שתופיע אחרי ניצחון על הבוס")]
    [SerializeField] private GameObject exitDoor;

    private int currentHits = 0;
    private bool isDead = false;

    private void Start()
    {
        UpdateHPVisuals();

        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }

    // פגיעה של Joy כשהיא קופצת על הבוס מלמעלה
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        EmotionController emotionController =
            collision.gameObject.GetComponent<EmotionController>();

        if (emotionController == null) return;

        // בודק שהשחקן במצב Joy
        if (emotionController.current != EmotionController.Emotion.Joy) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // אם השחקן פגע בבוס מלמעלה
            if (contact.normal.y < -0.5f)
            {
                OnBreak();

                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
                }

                break;
            }
        }
    }

    // נקרא כשהשחקן פוגע בבוס
    public void OnBreak()
    {
        if (isDead) return;

        currentHits++;

        Debug.Log("Mini Boss Hit: " + currentHits + "/" + hitsToKill);

        UpdateHPVisuals();

        if (currentHits < hitsToKill)
        {
            return;
        }

        Die();
    }

    private void UpdateHPVisuals()
    {
        for (int i = 0; i < hpVisuals.Length; i++)
        {
            if (hpVisuals[i] == null) continue;

            hpVisuals[i].SetActive(i >= currentHits);
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Mini Boss Defeated!");

        SpawnDebris();

        if (elevator != null)
        {
            elevator.UnlockReturnDown();
        }

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Exit Door is not connected to Level3MiniBossHealth");
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

                rb.AddTorque(
                    Random.Range(-torque, torque),
                    ForceMode2D.Impulse
                );
            }
        }
    }
}