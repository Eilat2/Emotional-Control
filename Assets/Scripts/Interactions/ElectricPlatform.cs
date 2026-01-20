using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] Stamina.StaminaType staminaTypeToDrain = Stamina.StaminaType.Joy;
    [SerializeField] float drainAmount = 15f;
    [SerializeField] float hitCooldown = 0.4f;

    [Header("Knockback")]
    [SerializeField] float knockbackForceX = 6f;
    [SerializeField] float knockbackForceY = 2f;

    [Header("Underside Check")]
   

    float nextHitTime = 0f;
    Collider2D myCol;

    void Awake()
    {
        myCol = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryShock(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryShock(collision);
    }

    void TryShock(Collision2D collision)
    {
        if (Time.time < nextHitTime) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (myCol == null) return;

        nextHitTime = Time.time + hitCooldown;

        // 1) Drain stamina
        Stamina[] staminas = collision.collider.GetComponents<Stamina>();
        for (int i = 0; i < staminas.Length; i++)
        {
            if (staminas[i].type == staminaTypeToDrain)
            {
                staminas[i].Use(drainAmount);
                break;
            }
        }

        // Flash – התחשמלות ויזואלית
        FlashOnHit flash = collision.gameObject.GetComponent<FlashOnHit>();
        if (flash != null)
        {
            flash.Flash();
        }

        // 2) Knockback
        Rigidbody2D rb = collision.rigidbody;
        if (rb != null)
        {
            float dir = (collision.transform.position.x < transform.position.x) ? -1f : 1f;

            Vector2 v = rb.linearVelocity;
            v.x = dir * knockbackForceX;
            v.y = Mathf.Max(v.y, knockbackForceY);
            rb.linearVelocity = v;
        }
    }
}
