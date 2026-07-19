using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private Stamina.StaminaType staminaType;

    private Image _fillImage;

    private void Awake()
    {
        _fillImage = GetComponent<Image>();
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
        if (type != staminaType || _fillImage == null)
            return;

        _fillImage.fillAmount = current / max;
    }
}
