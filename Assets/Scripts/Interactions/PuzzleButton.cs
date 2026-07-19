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

    private bool _playerInside;
    private EmotionController _playerEmotion;

    private bool _wasPressed;
    private bool _pressedCorrectly;

    private SpriteRenderer _sr;
    private Vector3 _originalScale;

    public bool WasPressed => _wasPressed;
    public bool PressedCorrectly => _pressedCorrectly;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;

        if (_sr != null && normalSprite != null)
            _sr.sprite = normalSprite;

        if (glowRenderer != null)
            glowRenderer.gameObject.SetActive(false);

        if (interactionPopup != null)
            interactionPopup.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInside || _playerEmotion == null)
            return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            PressButton();
    }

    private void PressButton()
    {
        // לא מאפשרים ללחוץ על אותו כפתור פעמיים
        if (_wasPressed)
            return;

        // אם הפאזל כבר נפתר, לא מאפשרים עוד לחיצות
        if (levelManager != null && levelManager.PuzzleSolved)
            return;

        EmotionType currentEmotion = _playerEmotion.GetCurrentEmotion();
        _wasPressed = true;
        _pressedCorrectly = currentEmotion == requiredEmotion;

        StateLogger.Log(nameof(PuzzleButton), _pressedCorrectly ? "Correct button press." : "Wrong button press.");

        if (_sr != null && pressedSprite != null)
            _sr.sprite = pressedSprite;

        // מקטינים קצת את הכפתור כדי לתת תחושת לחיצה
        transform.localScale = _originalScale * pressScale;

        ActivateGlow(currentEmotion);

        if (interactionPopup != null)
            interactionPopup.SetActive(false);

        if (levelManager != null)
            levelManager.RegisterButtonPress(this, currentEmotion, _pressedCorrectly);
        else
            Debug.LogWarning("PuzzleButton: Puzzle manager is not assigned.");
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

        _playerInside = true;
        _playerEmotion = other.GetComponentInParent<EmotionController>();

        StateLogger.Log(nameof(PuzzleButton), "Player entered button area.");

        // מציגים פופאפ רק אם הכפתור עדיין לא נלחץ
        if (interactionPopup != null && !_wasPressed)
            interactionPopup.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerInside = false;
        _playerEmotion = null;

        StateLogger.Log(nameof(PuzzleButton), "Player left button area.");

        if (interactionPopup != null)
            interactionPopup.SetActive(false);
    }
}
