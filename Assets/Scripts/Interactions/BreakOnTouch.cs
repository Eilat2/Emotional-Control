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
    public float vfxDestroyAfter = 1.5f;    // אחרי כמה זמן למחוק את ה-VFX

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
            SpawnVfxAt(breakableInRange.transform.position);

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
