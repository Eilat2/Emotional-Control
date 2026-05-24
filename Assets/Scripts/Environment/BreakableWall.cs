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

    [Header("Reveal")]
    [SerializeField] GameObject hiddenButton;       // כפתור שמוסתר מאחורי הקיר

    [Header("Tutorial")]
    [SerializeField] GameObject tutorialPopup;      // הפופאפ של הטוטוריאל (אם קיים)

    [Header("Puzzle Piece Unlock")]
    [SerializeField] private Level3PuzzlePiecePickup puzzlePieceToUnlock;
    // חלק הפאזל שייפתח לאיסוף רק אחרי שהאבן נשברת

    // מונע שבירה כפולה של אותו קיר
    private bool isBroken = false;

    // פונקציה שמופעלת כשהשחקן שובר את הקיר
    public void OnBreak()
    {
        // אם הקיר כבר נשבר - לא עושים כלום
        if (isBroken) return;

        isBroken = true;

        SpawnDebris(); // יוצרים שברים

        // אם יש פופאפ מחובר - נכבה אותו
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(false);
        }

        // אם יש כפתור מוסתר - נחשוף אותו
        if (hiddenButton != null)
        {
            hiddenButton.SetActive(true);
        }

        // פותח את האפשרות לאסוף את חלק הפאזל
        // זה יקרה רק אחרי שזעם באמת שבר את האבן
        if (puzzlePieceToUnlock != null)
        {
            puzzlePieceToUnlock.UnlockAfterStoneBroken();
        }

        // מוחקים את הקיר
        Destroy(gameObject);
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