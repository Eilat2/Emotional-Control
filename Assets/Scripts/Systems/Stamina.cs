using UnityEngine;

public class Stamina : MonoBehaviour
{
    public enum StaminaType
    {
        Joy,
        Rage
    }

    public StaminaType type = StaminaType.Joy;

    public float maxStamina = 100f;
    public float currentStamina;

    [Header("טעינה (Regen)")]
    public float regenPerSecond = 1f;
    public float regenDelay = 0.4f;

    [Header("זיהוי חוסר תזוזה")]
    public float idleSpeedThreshold = 0.05f;

    private Rigidbody2D rb;
    private float lastUseTime;
    private PlayerEmotionContext emotionContext;

    private bool depletedTriggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        emotionContext = GetComponent<PlayerEmotionContext>();
    }

    void Start()
    {
        // בכל סצנה חדשה הסטאמינה מתחילה מלאה
        ResetForNewScene();
    }

    void Update()
    {
        RegenerateWhenIdle();
    }

    void RegenerateWhenIdle()
    {
        if (currentStamina >= maxStamina)
            return;

        if (Time.time < lastUseTime + regenDelay)
            return;

        if (!IsIdle())
            return;

        currentStamina += regenPerSecond * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);

        GameEvents.RaiseStaminaChanged(type, currentStamina, maxStamina);
    }

    bool IsIdle()
    {
        if (rb == null) return true;

        return rb.linearVelocity.magnitude <= idleSpeedThreshold;
    }

    public bool Use(float amount)
    {
        if (currentStamina <= 0f)
        {
            TriggerDepleted();
            return false;
        }

        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0f);

        lastUseTime = Time.time;

        GameEvents.RaiseStaminaChanged(type, currentStamina, maxStamina);

        if (currentStamina <= 0f)
        {
            TriggerDepleted();
            return false;
        }

        return true;
    }

    private void TriggerDepleted()
    {
        if (depletedTriggered)
            return;

        depletedTriggered = true;

        if (emotionContext != null)
        {
            emotionContext.OnStaminaDepleted();
        }
    }

    public void Refill()
    {
        currentStamina = maxStamina;
        depletedTriggered = false;

        GameEvents.RaiseStaminaChanged(type, currentStamina, maxStamina);
    }

    public void ResetForNewScene()
    {
        // איפוס מלא להתחלה נקייה של כל סצנה
        currentStamina = maxStamina;
        lastUseTime = 0f;
        depletedTriggered = false;

        GameEvents.RaiseStaminaChanged(type, currentStamina, maxStamina);
    }
}