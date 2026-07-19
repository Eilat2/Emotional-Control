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

    private Rigidbody2D _rb;
    private float _lastUseTime;
    private PlayerEmotionContext _emotionContext;

    private bool _depletedTriggered;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _emotionContext = GetComponent<PlayerEmotionContext>();
    }

    private void Start()
    {
        // בכל סצנה חדשה הסטאמינה מתחילה מלאה
        ResetForNewScene();
    }

    private void Update()
    {
        RegenerateWhenIdle();
    }

    private void RegenerateWhenIdle()
    {
        if (currentStamina >= maxStamina)
            return;

        if (Time.time < _lastUseTime + regenDelay)
            return;

        if (!IsIdle())
            return;

        currentStamina += regenPerSecond * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);

        GameEvents.RaiseStaminaChanged(type, currentStamina, maxStamina);
    }

    private bool IsIdle()
    {
        if (_rb == null)
            return true;

        return _rb.linearVelocity.magnitude <= idleSpeedThreshold;
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

        _lastUseTime = Time.time;

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
        if (_depletedTriggered)
            return;

        _depletedTriggered = true;
        _emotionContext?.OnStaminaDepleted();
    }

    public void Refill()
    {
        SetStamina(maxStamina);
    }

    public void ResetForNewScene()
    {
        // איפוס מלא להתחלה נקייה של כל סצנה
        _lastUseTime = 0f;
        SetStamina(maxStamina);
    }

    public void ForceDeplete()
    {
        SetStamina(0f);
        TriggerDepleted();
    }

    private void SetStamina(float value)
    {
        currentStamina = value;
        _depletedTriggered = false;

        GameEvents.RaiseStaminaChanged(type, currentStamina, maxStamina);
    }
}
