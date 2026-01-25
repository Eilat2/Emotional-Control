using UnityEngine;

// קיר שביר שמגיב ל-OnBreak
public class BreakableWall : MonoBehaviour, IBreakable
{
    [Header("Debris Settings")]
    [SerializeField] GameObject debrisPiecePrefab; // Prefab של חתיכת קיר
    [SerializeField] int debrisCount = 12;          // כמה חתיכות יוצאות
    [SerializeField] float debrisForce = 4f;        // כוח הפיזור
    [SerializeField] float debrisTorque = 200f;     // סיבוב החתיכות
    [SerializeField] float debrisLifeTime = 1.5f;   // אחרי כמה זמן חתיכה נעלמת

    // פונקציה שמופעלת כשהשחקן שובר את הקיר
    public void OnBreak()
    {
        SpawnDebris();          // יוצרים שברים
        Destroy(gameObject);    // מוחקים את הקיר
    }

    // יצירת שברי קיר
    void SpawnDebris()
    {
        if (debrisPiecePrefab == null) return;

        for (int i = 0; i < debrisCount; i++)
        {
            // פיזור קטן מסביב לנקודת השבירה
            Vector3 offset = Random.insideUnitCircle * 0.15f;

            GameObject piece = Instantiate(
                debrisPiecePrefab,
                transform.position + offset,
                Quaternion.identity
            );

            // מוחקים את החתיכה אחרי זמן קצר
            Destroy(piece, debrisLifeTime);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // נותנים "פיצוץ" קטן
                rb.AddForce(Random.insideUnitCircle.normalized * debrisForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-debrisTorque, debrisTorque));
            }
        }
    }
}
