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
        // ממירים את ה-MonoBehaviour ל-interface
        neutralStrategy = neutralStrategyBehaviour as IEmotionStrategy;
        joyStrategy = joyStrategyBehaviour as IEmotionStrategy;
        rageStrategy = rageStrategyBehaviour as IEmotionStrategy;

        // אם אחת ההמרות נכשלה - זה אומר שגררת קומפוננטה שלא מממשת IEmotionStrategy
        if (neutralStrategy == null || joyStrategy == null || rageStrategy == null)
        {
            Debug.LogError("PlayerEmotionContext: אחד ה-Strategies לא מממש IEmotionStrategy. בדקי מה גררת בשדות.");
        }
    }

    void Update()
    {
        if (currentStrategy == null) return;

        // מעבירים תנועה (גם אם 0)
        currentStrategy.HandleMove(moveInput);

        // מעבירים מצב Space (לחיצה/החזקה/שחרור)
        currentStrategy.HandleJumpBreak(jumpHeld, pressedThisFrame, releasedThisFrame);

        // Tick של האסטרטגיה (פיזיקה/לוגיקה)
        currentStrategy.Tick();

        // חשוב: pressed/released צריכים להיות true רק פריים אחד
        pressedThisFrame = false;
        releasedThisFrame = false;
    }

    // ---------- Input System (Send Messages) ----------
    // חייב להיקרא OnMove כי ה-Action נקרא "Move"
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // חייב להיקרא OnJump_Break כי ה-Action נקרא "Jump_Break"
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

        if (next == currentStrategy) return;

        currentStrategy?.Exit();
        currentStrategy = next;
        currentStrategy?.Enter();
    }
}
