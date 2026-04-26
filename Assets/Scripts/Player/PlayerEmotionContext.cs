using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerEmotionContext : MonoBehaviour
{
    [Header("Strategies (גררי לכאן קומפוננטות מה-Player)")]
    [SerializeField] private MonoBehaviour neutralStrategyBehaviour;
    [SerializeField] private MonoBehaviour joyStrategyBehaviour;
    [SerializeField] private MonoBehaviour rageStrategyBehaviour;

    [Header("Visual")]
    [SerializeField] private PlayerVisualSwitcher visualSwitcher;

    private IEmotionStrategy neutralStrategy;
    private IEmotionStrategy joyStrategy;
    private IEmotionStrategy rageStrategy;

    private IEmotionStrategy currentStrategy;

    // קלט התנועה שמגיע מה-Input System
    private Vector2 moveInput;

    // מצב כפתור Jump_Break
    private bool jumpHeld = false;
    private bool pressedThisFrame = false;
    private bool releasedThisFrame = false;

    private void Awake()
    {
        // אם שכחנו לגרור באינספקטור, ננסה למצוא אוטומטית על השחקן/ילדים שלו
        if (visualSwitcher == null)
        {
            visualSwitcher = GetComponentInChildren<PlayerVisualSwitcher>();
        }

        // המרה מהשדות באינספקטור ל-interface של האסטרטגיות
        neutralStrategy = neutralStrategyBehaviour as IEmotionStrategy;
        joyStrategy = joyStrategyBehaviour as IEmotionStrategy;
        rageStrategy = rageStrategyBehaviour as IEmotionStrategy;

        // בדיקה שכל האסטרטגיות באמת קיימות ומממשות IEmotionStrategy
        if (neutralStrategy == null || joyStrategy == null || rageStrategy == null)
        {
            Debug.LogError("PlayerEmotionContext: אחד ה-Strategies לא מממש IEmotionStrategy או לא שוייך באינספקטור.");
        }

        if (visualSwitcher == null)
        {
            Debug.LogWarning("PlayerEmotionContext: לא נמצא PlayerVisualSwitcher. הדמות לא תתהפך ימינה/שמאלה.");
        }
    }

    private void Start()
    {
        // מתחילים תמיד ברגש ניטרלי
        if (currentStrategy == null)
        {
            SetEmotion(EmotionController.Emotion.Neutral);
        }
    }

    private void Update()
    {
        // אם המשחק בפאוז — לא נותנים לשחקן לזוז / לקפוץ / לשבור
        if (Time.timeScale == 0f)
            return;

        if (currentStrategy == null)
            return;

        // שליחת התנועה לאסטרטגיה הפעילה
        currentStrategy.HandleMove(moveInput);

        // עדכון כיוון הדמות לפי תנועה ימינה/שמאלה
        if (visualSwitcher != null)
        {
            visualSwitcher.SetDirection(moveInput.x);
        }

        // שליחת קלט קפיצה/שבירה לאסטרטגיה הפעילה
        currentStrategy.HandleJumpBreak(jumpHeld, pressedThisFrame, releasedThisFrame);

        // איפוס לחיצה/שחרור כדי שיהיו פעילים רק פריים אחד
        pressedThisFrame = false;
        releasedThisFrame = false;
    }

    private void FixedUpdate()
    {
        // אם המשחק בפאוז — לא מריצים לוגיקת פיזיקה של הרגש
        if (Time.timeScale == 0f)
            return;

        if (currentStrategy == null)
            return;

        // הפעלת הלוגיקה הפיזיקלית של האסטרטגיה
        currentStrategy.Tick();
    }

    public void OnMove(InputValue value)
    {
        // אם המשחק בפאוז — מאפסים תנועה כדי שלא תישמר לחיצה ישנה
        if (Time.timeScale == 0f)
        {
            moveInput = Vector2.zero;
            return;
        }

        // קריאת תנועה מה-Input System
        moveInput = value.Get<Vector2>();
    }

    public void OnJump_Break(InputValue value)
    {
        // אם המשחק בפאוז — מתעלמים מקפיצה/שבירה
        if (Time.timeScale == 0f)
            return;

        bool pressed = value.isPressed;

        // התחלת לחיצה
        if (pressed && !jumpHeld)
            pressedThisFrame = true;

        // שחרור לחיצה
        if (!pressed && jumpHeld)
            releasedThisFrame = true;

        // שמירת מצב הכפתור
        jumpHeld = pressed;
    }

    public void SetEmotion(EmotionController.Emotion e)
    {
        IEmotionStrategy next =
            e == EmotionController.Emotion.Joy ? joyStrategy :
            e == EmotionController.Emotion.Rage ? rageStrategy :
                                                  neutralStrategy;

        if (next == null)
        {
            Debug.LogError("PlayerEmotionContext: next strategy יצא null.");
            return;
        }

        // אם כבר נמצאים באותו רגש, לא עושים כלום
        if (next == currentStrategy)
            return;

        // יציאה מהרגש הקודם
        currentStrategy?.Exit();

        // מעבר לרגש החדש
        currentStrategy = next;

        // כניסה לרגש החדש
        currentStrategy?.Enter();
    }
}