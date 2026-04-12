using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{
    [Header("Stamina")]
    // איזה סוג סטאמינה להוריד
    [SerializeField] Stamina.StaminaType staminaTypeToDrain = Stamina.StaminaType.Joy;

    // כמה סטאמינה להוריד בכל פגיעה
    [SerializeField] float drainAmount = 15f;

    // זמן המתנה בין פגיעות
    [SerializeField] float hitCooldown = 0.4f;

    [Header("Slam Down")]
    // עוצמת הדחיפה כלפי מטה
    [SerializeField] float slamDownForce = 12f;

    // רפרנס לסקריפט של רעידת מצלמה
    private CameraShake cameraShake;

    // שומר מתי מותר לפגוע שוב
    float nextHitTime = 0f;

    void Start()
    {
        // מחפש את המצלמה הראשית ולוקח ממנה את סקריפט ה-CameraShake
        if (Camera.main != null)
        {
            cameraShake = Camera.main.GetComponent<CameraShake>();
        }
    }

    // נקרא כשהשחקן נכנס ל־Trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        TryShock(collision);
    }

    // נקרא כל עוד השחקן בתוך ה־Trigger
    void OnTriggerStay2D(Collider2D collision)
    {
        TryShock(collision);
    }

    // הפונקציה שמטפלת בלוגיקת החשמול
    void TryShock(Collider2D collision)
    {
        // אם עדיין לא עבר זמן ה-cooldown – לא פוגעים שוב
        if (Time.time < nextHitTime) return;

        // לוקחים את אובייקט השחקן הראשי דרך ה-Rigidbody אם יש
        GameObject playerObject = collision.attachedRigidbody != null
            ? collision.attachedRigidbody.gameObject
            : collision.gameObject;

        // בודקים שזה באמת השחקן
        if (!playerObject.CompareTag("Player")) return;

        // מעדכנים זמן לפגיעה הבאה
        nextHitTime = Time.time + hitCooldown;

        // ---------- 1) הורדת סטאמינה ----------
        Stamina[] staminas = playerObject.GetComponents<Stamina>();

        for (int i = 0; i < staminas.Length; i++)
        {
            if (staminas[i].type == staminaTypeToDrain)
            {
                staminas[i].Use(drainAmount);
                break;
            }
        }

        // ---------- 2) אפקט פגיעה ----------
        PlayerHitFeedback hitFeedback = playerObject.GetComponent<PlayerHitFeedback>();

        if (hitFeedback != null)
        {
            hitFeedback.PlayHitFeedback();
        }

        // ---------- 2.5) ניצוצות חשמל ----------
        ElectricVFX electricVFX = playerObject.GetComponent<ElectricVFX>();

        if (electricVFX != null)
        {
            electricVFX.PlaySparks();
        }

        // ---------- 2.6) ברק חשמל ----------
        LightningVFX lightning = playerObject.GetComponentInChildren<LightningVFX>();

        if (lightning != null)
        {
            lightning.PlayLightning();
        }

        // ---------- 3) סלאם דאון ----------
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 v = rb.linearVelocity;
            v.y = -slamDownForce;
            rb.linearVelocity = v;
        }

        // ---------- 4) רעידת מצלמה ----------
        if (cameraShake != null)
        {
            cameraShake.Shake();
        }
    }
}