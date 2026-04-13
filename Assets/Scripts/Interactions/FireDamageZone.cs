using UnityEngine;

// אזור אש שמוריד סטאמינה של שמחה ושל זעם כל עוד השחקן נמצא בתוכו
public class FireDamageZone : MonoBehaviour
{
    [Header("Stamina Drain")]
    // כמה סטאמינה יורדת בכל שנייה
    [SerializeField] private float drainPerSecond = 10f;

    // האם השחקן כרגע נמצא בתוך האש
    private bool playerInside = false;

    // רפרנסים לסטאמינה של שמחה וזעם
    private Stamina joyStamina;
    private Stamina rageStamina;

    void Update()
    {
        // אם השחקן לא בתוך האש - לא עושים כלום
        if (!playerInside)
            return;

        // מחשבים כמה להוריד בפריים הנוכחי לפי הזמן שעבר
        float drainAmount = drainPerSecond * Time.deltaTime;

        // מורידים סטאמינה של שמחה אם קיימת
        if (joyStamina != null)
        {
            joyStamina.Use(drainAmount);
        }

        // מורידים סטאמינה של זעם אם קיימת
        if (rageStamina != null)
        {
            rageStamina.Use(drainAmount);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // בודקים שרק השחקן מפעיל את האזור
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        // מאפסים רפרנסים קודמים ליתר ביטחון
        joyStamina = null;
        rageStamina = null;

        // מחפשים את כל רכיבי הסטאמינה על השחקן ועל ילדים שלו
        Stamina[] staminas = other.GetComponentsInChildren<Stamina>();

        // מאתרים איזו סטאמינה שייכת לשמחה ואיזו לזעם
        foreach (Stamina stamina in staminas)
        {
            if (stamina.type == Stamina.StaminaType.Joy)
            {
                joyStamina = stamina;
            }
            else if (stamina.type == Stamina.StaminaType.Rage)
            {
                rageStamina = stamina;
            }
        }

        Debug.Log("Player entered fire zone.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // בודקים שרק השחקן יוצא מהאזור
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        // מנקים רפרנסים כשעוזבים את האש
        joyStamina = null;
        rageStamina = null;

        Debug.Log("Player left fire zone.");
    }
}