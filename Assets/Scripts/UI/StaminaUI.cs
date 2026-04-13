using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private Stamina.StaminaType staminaType; // סוג הסטאמינה שהפס הזה מציג

    private Stamina stamina; // הסטאמינה שנמצאה בפועל
    private Slider slider;

    void Awake()
    {
        // לוקחים את ה-Slider שעל אותו אובייקט
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        // ניסיון ראשוני למצוא את הסטאמינה המתאימה
        FindStamina();
    }

    void Update()
    {
        // אם משום מה אין רפרנס לסטאמינה, מנסים למצוא שוב
        if (stamina == null)
        {
            FindStamina();
            return;
        }

        // אם המקסימום השתנה - נעדכן גם את הפס
        if (slider.maxValue != stamina.maxStamina)
            slider.maxValue = stamina.maxStamina;

        // מעדכנים את ערך הפס לפי הסטאמינה הנוכחית
        slider.value = stamina.currentStamina;
    }

    void FindStamina()
    {
        // מחפשים את השחקן לפי Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("StaminaUI: Player not found.");
            return;
        }

        // מחפשים את כל רכיבי הסטאמינה שעל השחקן
        Stamina[] staminas = player.GetComponents<Stamina>();

        foreach (Stamina s in staminas)
        {
            if (s.type == staminaType)
            {
                stamina = s;

                // מגדירים את הסליידר לפי נתוני הסטאמינה
                slider.minValue = 0f;
                slider.maxValue = stamina.maxStamina;
                slider.value = stamina.currentStamina;

                Debug.Log("StaminaUI connected successfully.");
                return;
            }
        }

        Debug.LogWarning("StaminaUI: Matching stamina type was not found.");
    }
}