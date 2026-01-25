using UnityEngine;
using UnityEngine.InputSystem;

// אחראי על שבירה / הריגה בלחיצת Rage
public class BreakOnTouch : MonoBehaviour
{
    private EmotionController emotion;   // בודק באיזה רגש אנחנו
    private Stamina rageStamina;         // סטאמינה של Rage בלבד

    [SerializeField] float breakCost = 20f; // כמה סטאמינה יורדת על פעולה

    private IBreakable breakableInRange; // מה שנמצא כרגע בטווח
    private InputAction jumpBreakAction; // האקשן של Space (Jump_Break)

    void Awake()
    {
        // הסקריפט יושב על BreakZone (Child)
        // אז את כל הרפרנסים לוקחים מהאבא (Player)
        Transform playerRoot = transform.root;

        emotion = playerRoot.GetComponent<EmotionController>();
        rageStamina = GetStaminaFrom(playerRoot, Stamina.StaminaType.Rage);

        PlayerInput playerInput = playerRoot.GetComponent<PlayerInput>();
        if (playerInput != null)
            jumpBreakAction = playerInput.actions["Jump_Break"];
        else
            Debug.LogError("BreakOnTouch: PlayerInput לא נמצא על השחקן");
    }

    void Update()
    {
        // אם אין משהו בטווח – אין מה לעשות
        if (breakableInRange == null) return;

        // חייבים להיות במצב Rage
        if (emotion != null && emotion.current != EmotionController.Emotion.Rage)
            return;

        // לחיצה על Space (Jump_Break)
        if (jumpBreakAction != null && jumpBreakAction.WasPressedThisFrame())
        {
            // אם אין מספיק סטאמינה – לא מבצעים
            if (rageStamina != null && !rageStamina.Use(breakCost))
                return;

            // אומרים לאובייקט: "תטפל בעצמך"
            breakableInRange.OnBreak();
            breakableInRange = null;
        }
    }

    // כשמשהו נכנס לטווח
    void OnTriggerEnter2D(Collider2D other)
    {
        // מחפשים קומפוננטה IBreakable על האובייקט או על ההורים שלו
        IBreakable breakable = other.GetComponentInParent<IBreakable>();
        if (breakable == null) return;

        Debug.Log("IN BREAK RANGE");
        breakableInRange = breakable;
    }

    // כשמשהו יוצא מהטווח
    void OnTriggerExit2D(Collider2D other)
    {
        IBreakable breakable = other.GetComponentInParent<IBreakable>();
        if (breakable == null) return;

        if (breakableInRange == breakable)
            breakableInRange = null;
    }

    // חיפוש סטאמינה לפי סוג
    Stamina GetStaminaFrom(Transform root, Stamina.StaminaType wantedType)
    {
        Stamina[] staminas = root.GetComponents<Stamina>();
        foreach (Stamina s in staminas)
            if (s.type == wantedType)
                return s;

        return null;
    }
}
