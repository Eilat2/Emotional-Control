using UnityEngine;

public class BreakOnTouch : MonoBehaviour
{
    private EmotionController emotion;

    // הסטאמינה של זעם בלבד
    private Stamina rageStamina;

    // כמה סטאמינה יורדת על כל שבירה
    public float breakCost = 20f;

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

        // אם אין סטאמינה של Rage – לא שוברים
        if (rageStamina != null && !rageStamina.Use(breakCost))
            return;

        // שבירה של אובייקטים עם תג Breakable
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Debug.Log("Broke breakable!");
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// מחפש על ה-Player סטאמינה לפי סוג (Joy / Rage)
    /// </summary>
    Stamina GetStamina(Stamina.StaminaType wantedType)
    {
        // מקבלים את כל ה-Stamina שעל ה-Player
        Stamina[] staminas = GetComponents<Stamina>();

        // מחפשים את זו שמתאימה לסוג שביקשנו
        foreach (Stamina s in staminas)
        {
            if (s.type == wantedType)
                return s;
        }

        // אם לא נמצאה סטאמינה מתאימה
        return null;
    }
}
