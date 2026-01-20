using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("חיבורים ידניים")]
    [SerializeField] GameObject player;              // הפלייר שעליו יושבות הסטאמינות
    [SerializeField] Stamina.StaminaType staminaType; // Joy או Rage – לבחור באינספקטור

    private Stamina stamina;               // הסטאמינה שנמצאה בפועל
    private Slider slider;

    void Awake()
    {
        // לוקחים את ה-Slider שעל אותו אובייקט
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        // בדיקה חשובה – שלא שכחנו לחבר Player
        if (player == null)
        {
            Debug.LogError("StaminaUI: לא חיברת Player באינספקטור");
            enabled = false;
            return;
        }

        // מוצאים על ה-Player את ה-Stamina מהסוג שבחרנו (Joy / Rage)
        Stamina[] staminas = player.GetComponents<Stamina>();
        foreach (Stamina s in staminas)
        {
            if (s.type == staminaType)
            {
                stamina = s;
                break;
            }
        }

        // אם לא נמצאה סטאמינה מהסוג שביקשנו – זו שגיאה
        if (stamina == null)
        {
            Debug.LogError($"StaminaUI: לא נמצאה Stamina מסוג {staminaType}");
            enabled = false;
            return;
        }

        // הגדרת הסליידר לפי נתוני הסטאמינה
        slider.minValue = 0f;
        slider.maxValue = stamina.maxStamina;
        slider.value = stamina.currentStamina;
    }

    void Update()
    {
        if (stamina == null) return;

        // אופציונלי: אם maxStamina משתנה, נשמור שהפס מתאים
        if (slider.maxValue != stamina.maxStamina)
            slider.maxValue = stamina.maxStamina;

        // כל פריים מעדכנים את הפס לפי הסטאמינה
        slider.value = stamina.currentStamina;
    }
}

