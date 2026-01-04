using UnityEngine;

public class BreakOnTouch : MonoBehaviour
{
    private EmotionController emotion;
    private Stamina stamina;

    public float breakCost = 20f; // כמה סטאמינה כל שבירה עולה

    void Awake()
    {
        emotion = GetComponent<EmotionController>();
        stamina = GetComponent<Stamina>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // אם אנחנו לא במצב Rage - לא שוברים
        if (emotion != null && emotion.current != EmotionController.Emotion.Rage)
            return;

        // אם אין מספיק סטאמינה - לא שוברים
        if (stamina != null && !stamina.Use(breakCost))
            return;

        if (collision.gameObject.CompareTag("Breakable"))
        {
            Debug.Log("Broke breakable!");
            Destroy(collision.gameObject);
        }
    }
}
