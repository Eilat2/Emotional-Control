using UnityEngine;

// מיני בוס של שלב 3
// דורש כמה פגיעות לפני שהוא מת
public class Level3MiniBossHealth : MonoBehaviour, IBreakable
{
    [Header("כמה פגיעות צריך כדי להרוג את הבוס")]
    [SerializeField] private int hitsToKill = 3;

    [Header("תצוגת חיים מעל הבוס")]
    [SerializeField] private GameObject[] hpVisuals;

    [Header("Debris")]
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int debrisCount = 8;
    [SerializeField] private float force = 5f;
    [SerializeField] private float torque = 5f;

    [Header("המעלית של שלב 3")]
    [SerializeField] private Level3SecretElevator elevator;

    [Header("הדלת שתופיע אחרי ניצחון על הבוס")]
    [SerializeField] private GameObject exitDoor;

    // כמה פגיעות הבוס כבר קיבל
    private int currentHits = 0;

    // מונע מצב שבו הבוס מת יותר מפעם אחת
    private bool isDead = false;

    private void Start()
    {
        // בהתחלה מציגים את כל החיים
        UpdateHPVisuals();

        // הדלת מוסתרת בתחילת הקרב
        // אם את מעדיפה לכבות ידנית באינספקטור, זה עדיין בסדר.
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }

    // נקרא כשהשחקן פוגע בבוס
    public void OnBreak()
    {
        // אם הבוס כבר מת, לא עושים כלום
        if (isDead) return;

        currentHits++;

        Debug.Log("Mini Boss Hit: " + currentHits + "/" + hitsToKill);

        // מעדכן את תצוגת החיים מעל הראש
        UpdateHPVisuals();

        // אם עדיין לא הגיע למספר הפגיעות הדרוש — לא מת
        if (currentHits < hitsToKill)
        {
            return;
        }

        // אם הגיע למספר הפגיעות הדרוש — מת
        Die();
    }

    // מעדכן אילו חיים מוצגים מעל הבוס
    private void UpdateHPVisuals()
    {
        for (int i = 0; i < hpVisuals.Length; i++)
        {
            if (hpVisuals[i] == null) continue;

            // מציג חיים רק אם האינדקס שלהם גדול/שווה לכמות הפגיעות
            // לדוגמה: אחרי פגיעה אחת HP_1 ייכבה, והשאר יישארו
            hpVisuals[i].SetActive(i >= currentHits);
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Mini Boss Defeated!");

        // יוצר שברים / אפקט מוות
        SpawnDebris();

        // מאפשר למעלית לרדת חזרה
        if (elevator != null)
        {
            elevator.UnlockReturnDown();
        }

        // מציג את הדלת לשלב הבא בחדר הראשי
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Exit Door is not connected to Level3MiniBossHealth");
        }

        // מוחק את הבוס מהסצנה
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