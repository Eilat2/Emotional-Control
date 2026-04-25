using UnityEngine;

// אזור אש שמוריד סטאמינה ומפעיל הבהוב
public class FireDamageZone : MonoBehaviour
{
    [Header("Stamina Drain")]
    [SerializeField] private float drainPerSecond = 10f;

    [Header("Hit Feedback")]
    [SerializeField] private float feedbackInterval = 0.2f;

    // האם השחקן כרגע בתוך האש
    private bool playerInside = false;

    // רפרנסים לסטאמינה של שמחה וזעם
    private Stamina joyStamina;
    private Stamina rageStamina;

    // רפרנס לסקריפט ההבהוב האמיתי שלך
    private PlayerHitFeedback playerHitFeedback;

    // מתי מותר להפעיל שוב הבהוב
    private float nextFeedbackTime = 0f;

    void Update()
    {
        if (!playerInside)
            return;

        float drainAmount = drainPerSecond * Time.deltaTime;

        if (joyStamina != null)
            joyStamina.Use(drainAmount);

        if (rageStamina != null)
            rageStamina.Use(drainAmount);

        if (playerHitFeedback != null && Time.time >= nextFeedbackTime)
        {
            playerHitFeedback.PlayHitFeedback();
            nextFeedbackTime = Time.time + feedbackInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        joyStamina = null;
        rageStamina = null;
        playerHitFeedback = null;

        Stamina[] staminas = other.GetComponentsInChildren<Stamina>();

        foreach (Stamina stamina in staminas)
        {
            if (stamina.type == Stamina.StaminaType.Joy)
                joyStamina = stamina;
            else if (stamina.type == Stamina.StaminaType.Rage)
                rageStamina = stamina;
        }

        playerHitFeedback = other.GetComponentInChildren<PlayerHitFeedback>();

        nextFeedbackTime = 0f;

        Debug.Log("Player entered fire zone.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        joyStamina = null;
        rageStamina = null;
        playerHitFeedback = null;

        Debug.Log("Player left fire zone.");
    }
}