using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerEmotionContext : MonoBehaviour
{
    [Header("Strategies (גררי לכאן קומפוננטות מה-Player)")]
    [SerializeField] private MonoBehaviour neutralStrategyBehaviour;
    [SerializeField] private MonoBehaviour joyStrategyBehaviour;
    [SerializeField] private MonoBehaviour rageStrategyBehaviour;

    private IEmotionStrategy neutralStrategy;
    private IEmotionStrategy joyStrategy;
    private IEmotionStrategy rageStrategy;

    private IEmotionStrategy currentStrategy;

    // קלט תנועה
    private Vector2 moveInput;

    // מצב כפתור Jump_Break (Space)
    private bool jumpHeld = false;
    private bool pressedThisFrame = false;
    private bool releasedThisFrame = false;

    void Awake()
    {
        neutralStrategy = neutralStrategyBehaviour as IEmotionStrategy;
        joyStrategy = joyStrategyBehaviour as IEmotionStrategy;
        rageStrategy = rageStrategyBehaviour as IEmotionStrategy;

        if (neutralStrategy == null || joyStrategy == null || rageStrategy == null)
        {
            Debug.LogError("PlayerEmotionContext: אחד ה-Strategies לא מממש IEmotionStrategy או לא שוייך באינספקטור. בדקי מה גררת בשדות.");
        }
    }

    void Start()
    {
        // כדי שלא נישאר עם currentStrategy = null בתחילת משחק
        // (EmotionController אמור לקרוא SetEmotion, אבל זה ביטוח)
        if (currentStrategy == null)
        {
            SetEmotion(EmotionController.Emotion.Neutral);
        }
    }

    // Update = קלט/אירועים של פריים (לא פיזיקה)
    void Update()
    {
        if (currentStrategy == null) return;

        // מעבירים תנועה (גם אם 0)
        currentStrategy.HandleMove(moveInput);

        // מעבירים מצב Space (לחיצה/החזקה/שחרור)
        currentStrategy.HandleJumpBreak(jumpHeld, pressedThisFrame, releasedThisFrame);

        // חשוב: pressed/released צריכים להיות true רק פריים אחד
        pressedThisFrame = false;
        releasedThisFrame = false;
    }

    // FixedUpdate = פיזיקה (תנועה עם Rigidbody2D)
    void FixedUpdate()
    {
        if (currentStrategy == null) return;

        // Tick של האסטרטגיה (פיזיקה/תנועה)
        currentStrategy.Tick();
    }

    // ---------- Input System (Send Messages) ----------
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump_Break(InputValue value)
    {
        bool pressed = value.isPressed;

        // pressedThisFrame = Down
        if (pressed && !jumpHeld)
            pressedThisFrame = true;

        // releasedThisFrame = Up
        if (!pressed && jumpHeld)
            releasedThisFrame = true;

        // מצב מוחזק
        jumpHeld = pressed;
    }
    // -----------------------------------------------

    // נקרא ע"י EmotionController כשהרגש משתנה
    public void SetEmotion(EmotionController.Emotion e)
    {
        IEmotionStrategy next =
            e == EmotionController.Emotion.Joy ? joyStrategy :
            e == EmotionController.Emotion.Rage ? rageStrategy :
                                                  neutralStrategy;

        if (next == null)
        {
            Debug.LogError("PlayerEmotionContext: next strategy יצא null. בדקי ששייכת את ה-Behaviour בשדות באינספקטור.");
            return;
        }

        if (next == currentStrategy) return;

        currentStrategy?.Exit();
        currentStrategy = next;
        currentStrategy?.Enter();
    }
}
