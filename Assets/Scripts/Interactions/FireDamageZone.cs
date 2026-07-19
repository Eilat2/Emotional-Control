using UnityEngine;

// אזור אש שמוריד סטאמינה רק לרגש הפעיל ומפעיל הבהוב
public class FireDamageZone : MonoBehaviour
{
    [Header("Stamina Drain")]
    [SerializeField] private float drainPerSecond = 10f;

    [Header("Hit Feedback")]
    [SerializeField] private float feedbackInterval = 0.2f;

    private bool _playerInside;

    private Stamina _joyStamina;
    private Stamina _rageStamina;
    private EmotionController _emotionController;
    private PlayerHitFeedback _playerHitFeedback;

    private float _nextFeedbackTime;

    private void Update()
    {
        if (!_playerInside || _emotionController == null)
            return;

        float drainAmount = drainPerSecond * Time.deltaTime;
        EmotionType currentEmotion = _emotionController.GetCurrentEmotion();

        if (currentEmotion == EmotionType.Joy && _joyStamina != null)
            _joyStamina.Use(drainAmount);
        else if (currentEmotion == EmotionType.Rage && _rageStamina != null)
            _rageStamina.Use(drainAmount);

        if (_playerHitFeedback != null && Time.time >= _nextFeedbackTime)
        {
            _playerHitFeedback.PlayHitFeedback();
            _nextFeedbackTime = Time.time + feedbackInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = true;

        _emotionController = other.GetComponentInParent<EmotionController>();
        _playerHitFeedback = other.GetComponentInChildren<PlayerHitFeedback>();

        _joyStamina = null;
        _rageStamina = null;

        Stamina[] staminas = other.GetComponentsInChildren<Stamina>();
        foreach (Stamina stamina in staminas)
        {
            if (stamina.type == Stamina.StaminaType.Joy)
                _joyStamina = stamina;
            else if (stamina.type == Stamina.StaminaType.Rage)
                _rageStamina = stamina;
        }

        _nextFeedbackTime = 0f;

        StateLogger.Log(nameof(FireDamageZone), "Player entered fire zone.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = false;

        _joyStamina = null;
        _rageStamina = null;
        _emotionController = null;
        _playerHitFeedback = null;

        StateLogger.Log(nameof(FireDamageZone), "Player left fire zone.");
    }
}
