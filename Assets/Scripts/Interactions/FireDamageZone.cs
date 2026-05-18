using UnityEngine;

// אזור אש שמוריד סטאמינה רק לרגש הפעיל ומפעיל הבהוב
public class FireDamageZone : MonoBehaviour
{
    [Header("Stamina Drain")]
    [SerializeField] private float drainPerSecond = 10f;

    [Header("Hit Feedback")]
    [SerializeField] private float feedbackInterval = 0.2f;

    private bool playerInside = false;

    private Stamina joyStamina;
    private Stamina rageStamina;
    private EmotionController emotionController;
    private PlayerHitFeedback playerHitFeedback;

    private float nextFeedbackTime = 0f;

    void Update()
    {
        if (!playerInside || emotionController == null)
            return;

        float drainAmount = drainPerSecond * Time.deltaTime;

        EmotionType currentEmotion = emotionController.GetCurrentEmotion();

        if (currentEmotion == EmotionType.Joy && joyStamina != null)
        {
            joyStamina.Use(drainAmount);
        }
        else if (currentEmotion == EmotionType.Rage && rageStamina != null)
        {
            rageStamina.Use(drainAmount);
        }

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
        emotionController = other.GetComponentInParent<EmotionController>();
        playerHitFeedback = other.GetComponentInChildren<PlayerHitFeedback>();

        Stamina[] staminas = other.GetComponentsInChildren<Stamina>();

        foreach (Stamina stamina in staminas)
        {
            if (stamina.type == Stamina.StaminaType.Joy)
                joyStamina = stamina;
            else if (stamina.type == Stamina.StaminaType.Rage)
                rageStamina = stamina;
        }

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
        emotionController = null;
        playerHitFeedback = null;

        Debug.Log("Player left fire zone.");
    }
}