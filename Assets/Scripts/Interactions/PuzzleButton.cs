using UnityEngine;
using UnityEngine.InputSystem;

// כפתור פאזל שאפשר ללחוץ עליו עם כל רגש,
// אבל רק רגש מסוים נחשב ללחיצה נכונה
public class PuzzleButton : MonoBehaviour
{
    [Header("Puzzle")]
    public EmotionType requiredEmotion;      // איזה רגש נדרש לכפתור הזה
    public PuzzleManager levelManager;       // מנהל הפאזל שבודק אם הכול נפתר

    [Header("Button Visual")]
    [SerializeField] private Sprite normalSprite;      // ספרייט רגיל
    [SerializeField] private Sprite pressedSprite;     // ספרייט לחוץ
    [SerializeField] private float pressScale = 0.9f;  // הקטנה בלחיצה

    [Header("Glow")]
    [SerializeField] private SpriteRenderer glowRenderer; // הילה סביב הכפתור
    [SerializeField] private Color joyGlowColor = new Color(1f, 0.9f, 0.2f, 0.85f);
    [SerializeField] private Color rageGlowColor = new Color(1f, 0.25f, 0.25f, 0.85f);
    [SerializeField] private Color neutralGlowColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);

    [Header("Interaction Popup")]
    [SerializeField] private GameObject interactionPopup; // הפופאפ "Press F"

    private bool playerInside = false;        // האם השחקן בתוך הטריגר
    private EmotionController playerEmotion;  // הרגש הנוכחי של השחקן

    private bool wasPressed = false;          // האם הכפתור כבר נלחץ
    private bool pressedCorrectly = false;    // האם הכפתור נלחץ עם הרגש הנכון

    private SpriteRenderer sr;
    private Vector3 originalScale;

    public bool WasPressed => wasPressed;
    public bool PressedCorrectly => pressedCorrectly;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        // מגדירים ספרייט רגיל בהתחלה
        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }

        // מכבים את ההילה בהתחלה
        if (glowRenderer != null)
        {
            glowRenderer.gameObject.SetActive(false);
        }

        // מכבים את הפופאפ בהתחלה
        if (interactionPopup != null)
        {
            interactionPopup.SetActive(false);
        }
    }

    private void Update()
    {
        // אם השחקן לא ליד הכפתור — לא עושים כלום
        if (!playerInside || playerEmotion == null)
            return;

        // בדיקה ללחיצה על F
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        // לא מאפשרים ללחוץ על אותו כפתור פעמיים
        if (wasPressed)
            return;

        // שומרים את הרגש הנוכחי של השחקן בזמן הלחיצה
        EmotionType currentEmotion = playerEmotion.GetCurrentEmotion();

        wasPressed = true;

        // בודקים אם הרגש של השחקן תואם לרגש שהכפתור דורש
        if (currentEmotion == requiredEmotion)
        {
            pressedCorrectly = true;
            Debug.Log("Correct button press.");
        }
        else
        {
            pressedCorrectly = false;
            Debug.Log("Wrong button press.");
        }

        // מחליפים לספרייט של כפתור לחוץ
        if (sr != null && pressedSprite != null)
        {
            sr.sprite = pressedSprite;
        }

        // מקטינים קצת את הכפתור כדי לתת תחושת לחיצה
        transform.localScale = originalScale * pressScale;

        // מפעילים הילה בצבע של הרגש שלחץ על הכפתור
        ActivateGlow(currentEmotion);

        // מכבים את הפופאפ אחרי הלחיצה
        if (interactionPopup != null)
        {
            interactionPopup.SetActive(false);
        }

        // מעדכנים את מנהל הפאזל כדי שיבדוק אם כל הכפתורים נלחצו נכון
        if (levelManager != null)
        {
            levelManager.CheckPuzzleState();
        }
        else
        {
            Debug.LogWarning("Puzzle manager is not assigned.");
        }
    }

    private void ActivateGlow(EmotionType emotion)
    {
        if (glowRenderer == null)
            return;

        glowRenderer.gameObject.SetActive(true);
        glowRenderer.color = GetGlowColor(emotion);
    }

    private Color GetGlowColor(EmotionType emotion)
    {
        switch (emotion)
        {
            case EmotionType.Joy:
                return joyGlowColor;

            case EmotionType.Rage:
                return rageGlowColor;

            case EmotionType.Neutral:
                return neutralGlowColor;

            default:
                return Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
        playerEmotion = other.GetComponentInParent<EmotionController>();

        Debug.Log("Player entered button area.");

        // מציגים פופאפ רק אם הכפתור עדיין לא נלחץ
        if (interactionPopup != null && !wasPressed)
        {
            interactionPopup.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        playerEmotion = null;

        Debug.Log("Player left button area.");

        // מסתירים את הפופאפ כשהשחקן מתרחק
        if (interactionPopup != null)
        {
            interactionPopup.SetActive(false);
        }
    }
}