using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{
    [Header("Stamina")]
    public Stamina.StaminaType staminaTypeToDrain = Stamina.StaminaType.Joy;
    public float drainAmount = 15f;
    public float hitCooldown = 0.4f;

    [Header("Knockback")]
    public float knockbackForceX = 6f;
    public float knockbackForceY = 2f;

    [Header("Underside Check")]
    public float undersideMargin = 0.05f; 

    private float nextHitTime = 0f;
    private Collider2D myCol;

    void Awake()
    {
        myCol = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryShock(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryShock(collision);
    }

    private void TryShock(Collision2D collision)
    {
        
        // Debug.Log("Collision with: " + collision.collider.name);

        if (Time.time < nextHitTime) return;
        if (!collision.collider.CompareTag("Player")) return;
        if (myCol == null) return;


        float undersideY = myCol.bounds.min.y;

        
        bool hitFromBelow = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 p = collision.GetContact(i).point;
            if (p.y <= undersideY + undersideMargin)
            {
                hitFromBelow = true;
                break;
            }
        }

        if (!hitFromBelow) return;

        nextHitTime = Time.time + hitCooldown;

        // 1) 
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
        FlashOnHit flash = collision.collider.GetComponent<FlashOnHit>();
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
