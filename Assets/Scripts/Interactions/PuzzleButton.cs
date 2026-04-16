using UnityEngine;
using UnityEngine.InputSystem;

// כפתור פאזל שאפשר ללחוץ עליו עם כל רגש,
// אבל רק רגש מסוים נחשב ללחיצה נכונה
public class PuzzleButton : MonoBehaviour
{
    [Header("Puzzle")]
    public EmotionType requiredEmotion;
    public LevelPipeManager levelManager;

    [Header("Button Visual")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private float pressScale = 0.9f;

    [Header("Glow")]
    [SerializeField] private SpriteRenderer glowRenderer;
    [SerializeField] private Color joyGlowColor = new Color(1f, 0.9f, 0.2f, 0.85f);
    [SerializeField] private Color rageGlowColor = new Color(1f, 0.25f, 0.25f, 0.85f);
    [SerializeField] private Color neutralGlowColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);

    private bool playerInside = false;
    private EmotionController playerEmotion;

    private bool wasPressed = false;
    private bool pressedCorrectly = false;

    private SpriteRenderer sr;
    private Vector3 originalScale;

    public bool WasPressed => wasPressed;
    public bool PressedCorrectly => pressedCorrectly;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }

        // מכבים את ההילה בהתחלה
        if (glowRenderer != null)
        {
            glowRenderer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInside || playerEmotion == null)
            return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        if (wasPressed)
            return;

        EmotionType currentEmotion = playerEmotion.GetCurrentEmotion();

        wasPressed = true;

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

        // מחליפים לספרייט לחוץ
        if (sr != null && pressedSprite != null)
        {
            sr.sprite = pressedSprite;
        }

        // מקטינים קצת לתחושת לחיצה
        transform.localScale = originalScale * pressScale;

        // מפעילים הילה בצבע של הרגש
        ActivateGlow(currentEmotion);

        // מודיעים למנהל לבדוק את מצב הפאזל
        if (levelManager != null)
        {
            levelManager.CheckPuzzleState();
        }
        else
        {
            Debug.LogWarning("Level manager is not assigned.");
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
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        playerEmotion = null;

        Debug.Log("Player left button area.");
    }
}