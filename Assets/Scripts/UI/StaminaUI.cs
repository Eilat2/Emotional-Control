using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private Stamina.StaminaType staminaType;

    private Image fillImage;

    void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        GameEvents.OnStaminaChanged += HandleStaminaChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnStaminaChanged -= HandleStaminaChanged;
    }

    private void HandleStaminaChanged(Stamina.StaminaType type, float current, float max)
    {
        if (type != staminaType)
            return;

        if (fillImage == null)
            return;

        fillImage.fillAmount = current / max;
    }
}