using UnityEngine;

public class Stamina : MonoBehaviour
{
    // סוג הסטאמינה – כדי שנדע אם היא שייכת לשמחה או לזעם
    public enum StaminaType
    {
        Joy,
        Rage
    }

    // קובעים באינספקטור האם זו סטאמינה של Joy או Rage
    public StaminaType type = StaminaType.Joy;

    // כמות הסטאמינה המקסימלית
    public float maxStamina = 100f;

    // כמות הסטאמינה הנוכחית (משתנה בזמן המשחק)
    public float currentStamina;

    void Start()
    {
        // בתחילת המשחק הסטאמינה מלאה
        currentStamina = maxStamina;
    }

    /// <summary>
    /// שימוש בסטאמינה
    /// amount = כמה להוריד
    /// מחזיר true אם עדיין נשארה סטאמינה, false אם נגמרה
    /// </summary>
    public bool Use(float amount)
    {
        // אם כבר אין סטאמינה – אי אפשר להשתמש
        if (currentStamina <= 0f)
            return false;

        // מורידים את הכמות שביקשו
        currentStamina -= amount;

        // דואגים שלא תרד מתחת ל-0
        currentStamina = Mathf.Max(currentStamina, 0f);

        // מחזיר האם עדיין יש סטאמינה
        return currentStamina > 0f;
    }

    /// <summary>
    /// מילוי מלא של הסטאמינה
    /// (לדוגמה: אחרי פסילה / Respawn)
    /// </summary>
    public void Refill()
    {
        currentStamina = maxStamina;
    }
}
