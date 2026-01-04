using UnityEngine;

public class Stamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;

    void Start()
    {
        currentStamina = maxStamina;
    }

    public bool Use(float amount)
    {
        if (currentStamina <= 0)
            return false;

        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0);
        return currentStamina > 0;
    }

    public void Refill()
    {
        currentStamina = maxStamina;
    }
}
