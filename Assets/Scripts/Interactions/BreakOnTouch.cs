using UnityEngine;
using UnityEngine.InputSystem;

public class BreakOnTouch : MonoBehaviour
{
    private EmotionController emotion;

    // הסטאמינה של זעם בלבד
    private Stamina rageStamina;

    // כמה סטאמינה יורדת על כל שבירה
    public float breakCost = 20f;

    [Header("VFX")]
    public ParticleSystem breakVfxPrefab;   // לגרור לכאן את BreakParticles (Prefab)
    public float vfxDestroyAfter = 3.5f;    // אחרי כמה זמן למחוק את ה-VFX

    // ====== ✅ ADDED: Debris (Real Pieces) ======
    [Header("Debris (Real Pieces)")]
    public GameObject debrisPiecePrefab;    // לגרור לכאן את DebrisPiece (Prefab)
    public int debrisCount = 12;            // כמה חתיכות יוצאות
    public float debrisForce = 4f;          // כוח פיזור
    public float debrisTorque = 200f;       // סיבוב חתיכות
    public float debrisLifeTime = 1.5f;       // אחרי כמה שניות חתיכה נעלמת
    // ===========================================

    // נשמור את האובייקט שאפשר לשבור כשאנחנו לידו
    private GameObject breakableInRange;

    // האקשן מה-Input Actions (Space) בשם Jump_Break
    private InputAction jumpBreakAction;

    void Awake()
    {
        // רפרנס לבקר הרגשות
        emotion = GetComponent<EmotionController>();

        // מוצאים את הסטאמינה מסוג Rage בלבד
        rageStamina = GetStamina(Stamina.StaminaType.Rage);

        // תופסים את האקשן מתוך PlayerInput
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            jumpBreakAction = playerInput.actions["Jump_Break"]; // השם עם קו תחתון
        }
        else
        {
            Debug.LogError("BreakOnTouch: No PlayerInput found on Player! (needed for Jump_Break action)");
        }
    }

    void Update()
    {
        // חייבים להיות ליד משהו שביר
        if (breakableInRange == null) return;

        // חייבים להיות במצב Rage
        if (emotion != null && emotion.current != EmotionController.Emotion.Rage)
            return;

        // רק בלחיצה על האקשן Jump_Break (Space)
        if (jumpBreakAction != null && jumpBreakAction.WasPressedThisFrame())
        {
            // אם אין מספיק סטאמינה של Rage – לא שוברים
            if (rageStamina != null && !rageStamina.Use(breakCost))
                return;

            // מפעילים Particles
            // SpawnVfxAt(breakableInRange.transform.position);

            // ====== ✅ ADDED: מפעילים שברים אמיתיים במקום Particles ======
            SpawnDebrisAt(breakableInRange.transform.position);
            // ===========================================================

            Debug.Log("Broke breakable with Jump_Break!");
            Destroy(breakableInRange);
            breakableInRange = null;
        }
    }

    // ✅ Trigger מה-BreakZone (Child)
    void OnTriggerEnter2D(Collider2D other)
    {
        // BreakZone הוא Child → הקיר הוא ה-Parent
        Transform wall = other.transform.parent;
        if (wall == null) return;

        // הקיר צריך להיות Breakable
        if (!wall.CompareTag("Breakable"))
            return;

        Debug.Log("IN BREAK RANGE: " + wall.gameObject.name);
        breakableInRange = wall.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Transform wall = other.transform.parent;
        if (wall == null) return;

        if (breakableInRange != null && wall.gameObject == breakableInRange)
        {
            Debug.Log("OUT OF BREAK RANGE: " + wall.gameObject.name);
            breakableInRange = null;
        }
    }

    // VFX לפי מיקום
    void SpawnVfxAt(Vector3 spawnPos)
    {
        if (breakVfxPrefab == null) return;

        spawnPos.z = 0f;

        ParticleSystem vfx = Instantiate(breakVfxPrefab, spawnPos, Quaternion.identity);
        vfx.Play();
        Destroy(vfx.gameObject, vfxDestroyAfter);
    }

    // ====== ✅ ADDED: שברים אמיתיים (GameObjects) לפי מיקום ======
    void SpawnDebrisAt(Vector3 spawnPos)
    {
        if (debrisPiecePrefab == null) return;

        spawnPos.z = 0f;

        for (int i = 0; i < debrisCount; i++)
        {
            // פיזור קטן סביב נקודת השבירה
            Vector3 offset = new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f),
                0f
            );

            GameObject piece = Instantiate(debrisPiecePrefab, spawnPos + offset, Quaternion.identity);

            // מוחקים אחרי זמן קצר כדי שלא יצטברו אובייקטים
            Destroy(piece, debrisLifeTime);

            Rigidbody2D rb2d = piece.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                // כוח אקראי אבל עם נטייה קצת למעלה כדי שיראה “פיצוץ”
                Vector2 dir = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(0.3f, 1f)
                ).normalized;

                rb2d.AddForce(dir * debrisForce, ForceMode2D.Impulse);
                rb2d.AddTorque(Random.Range(-debrisTorque, debrisTorque));
            }
        }
    }
    // ============================================================

    /// <summary>
    /// מחפש על ה-Player סטאמינה לפי סוג (Joy / Rage)
    /// </summary>
    Stamina GetStamina(Stamina.StaminaType wantedType)
    {
        Stamina[] staminas = GetComponents<Stamina>();

        foreach (Stamina s in staminas)
        {
            if (s.type == wantedType)
                return s;
        }

        return null;
    }
}
