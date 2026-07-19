using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{
    [Header("Stamina")]
    // איזה סוג סטאמינה להוריד
    [SerializeField] private Stamina.StaminaType staminaTypeToDrain = Stamina.StaminaType.Joy;

    // כמה סטאמינה להוריד בכל פגיעה
    [SerializeField] private float drainAmount = 15f;

    // זמן המתנה בין פגיעות
    [SerializeField] private float hitCooldown = 0.4f;

    [Header("Slam Down")]
    // עוצמת הדחיפה כלפי מטה
    [SerializeField] private float slamDownForce = 12f;

    // רפרנס לסקריפט של רעידת מצלמה
    private CameraShake _cameraShake;

    // שומר מתי מותר לפגוע שוב
    private float _nextHitTime;

    private void Start()
    {
        // מחפש את המצלמה הראשית ולוקח ממנה את סקריפט ה-CameraShake
        if (Camera.main != null)
            _cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    private void OnTriggerEnter2D(Collider2D collision) => TryShock(collision);
    private void OnTriggerStay2D(Collider2D collision) => TryShock(collision);

    private void TryShock(Collider2D collision)
    {
        if (Time.time < _nextHitTime)
            return;

        // לוקחים את אובייקט השחקן הראשי דרך ה-Rigidbody אם יש
        GameObject playerObject = collision.attachedRigidbody != null
            ? collision.attachedRigidbody.gameObject
            : collision.gameObject;

        if (!playerObject.CompareTag("Player"))
            return;

        _nextHitTime = Time.time + hitCooldown;

        DrainStamina(playerObject);
        PlayHitEffects(playerObject);
        SlamPlayerDown(playerObject);

        _cameraShake?.Shake();
    }

    private void DrainStamina(GameObject playerObject)
    {
        Stamina[] staminas = playerObject.GetComponents<Stamina>();

        foreach (Stamina stamina in staminas)
        {
            if (stamina.type == staminaTypeToDrain)
            {
                stamina.Use(drainAmount);
                break;
            }
        }
    }

    private void PlayHitEffects(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerHitFeedback>()?.PlayHitFeedback();
        playerObject.GetComponent<ElectricVFX>()?.PlaySparks();
        playerObject.GetComponentInChildren<LightningVFX>()?.PlayLightning();
    }

    private void SlamPlayerDown(GameObject playerObject)
    {
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Vector2 v = rb.linearVelocity;
        v.y = -slamDownForce;
        rb.linearVelocity = v;
    }
}
