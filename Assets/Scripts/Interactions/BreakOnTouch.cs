using UnityEngine;

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

    void Awake()
    {
        // רפרנס לבקר הרגשות
        emotion = GetComponent<EmotionController>();

        // מוצאים את הסטאמינה מסוג Rage בלבד
        rageStamina = GetStamina(Stamina.StaminaType.Rage);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // אם אנחנו לא במצב Rage – לא שוברים בכלל
        if (emotion != null && emotion.current != EmotionController.Emotion.Rage)
            return;

        // שוברים רק אובייקטים עם תג Breakable
        if (!collision.gameObject.CompareTag("Breakable"))
            return;

        // אם אין מספיק סטאמינה של Rage – לא שוברים
        // (אם rageStamina משום מה לא נמצא -> נשבור בלי סטאמינה, כדי שלא "יתקע" לך דיבאג)
        if (rageStamina != null && !rageStamina.Use(breakCost))
            return;

        // 💥 מפעילים Particles במקום השבירה
        SpawnVfx(collision);

        Debug.Log("Broke breakable!");
        Destroy(collision.gameObject);
    }

    void SpawnVfx(Collision2D collision)
    {
        if (breakVfxPrefab == null) return;

        // נקודת מגע (נראה יותר טוב מ-center)
        Vector3 spawnPos = collision.transform.position;

        // הגנה: לפעמים אין contacts (נדיר אבל קורה)
        if (collision.contactCount > 0)
            spawnPos = collision.GetContact(0).point;

        ParticleSystem vfx = Instantiate(breakVfxPrefab, spawnPos, Quaternion.identity);

        // חשוב ב-2D: לשים Z=0 כדי שלא "ייעלם" מאחורי המצלמה/אובייקטים
        Vector3 p = vfx.transform.position;
        p.z = 0f;
        vfx.transform.position = p;

        // אם בפראב יש Play On Awake כבוי - נריץ ידנית (לא מזיק גם אם דולק)
        vfx.Play();

        // מוחקים אחרי זמן קצר כדי שלא יצטברו אובייקטים
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
