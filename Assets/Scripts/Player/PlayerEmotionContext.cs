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

    private Vector2 moveInput;

    private bool jumpHeld = false;
    private bool pressedThisFrame = false;
    private bool releasedThisFrame = false;

    // 🔴 חדש – למנוע טריגר כפול
    private bool isFailing = false;

    private void Awake()
    {
        if (visualSwitcher == null)
        {
            visualSwitcher = GetComponentInChildren<PlayerVisualSwitcher>();
        }

        neutralStrategy = neutralStrategyBehaviour as IEmotionStrategy;
        joyStrategy = joyStrategyBehaviour as IEmotionStrategy;
        rageStrategy = rageStrategyBehaviour as IEmotionStrategy;

        if (neutralStrategy == null || joyStrategy == null || rageStrategy == null)
        {
            Debug.LogError("PlayerEmotionContext: אחד ה-Strategies לא מממש IEmotionStrategy או לא שוייך באינספקטור.");
        }

        if (visualSwitcher == null)
        {
            Debug.LogWarning("PlayerEmotionContext: לא נמצא PlayerVisualSwitcher.");
        }
    }

    private void Start()
    {
        if (currentStrategy == null)
        {
            SetEmotion(EmotionController.Emotion.Neutral);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f || isFailing)
            return;

        if (currentStrategy == null)
            return;

        currentStrategy.HandleMove(moveInput);

        if (visualSwitcher != null)
        {
            visualSwitcher.SetDirection(moveInput.x);
        }

        currentStrategy.HandleJumpBreak(jumpHeld, pressedThisFrame, releasedThisFrame);

        pressedThisFrame = false;
        releasedThisFrame = false;
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f || isFailing)
            return;

        if (currentStrategy == null)
            return;

        currentStrategy.Tick();
    }

    public void OnMove(InputValue value)
    {
        if (Time.timeScale == 0f || isFailing)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = value.Get<Vector2>();
    }

    public void OnJump_Break(InputValue value)
    {
        if (Time.timeScale == 0f || isFailing)
            return;

        bool pressed = value.isPressed;

        if (pressed && !jumpHeld)
            pressedThisFrame = true;

        if (!pressed && jumpHeld)
            releasedThisFrame = true;

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

        if (next == currentStrategy)
            return;

        currentStrategy?.Exit();

        currentStrategy = next;

        // מאפסים את מצב הקפיצה בכל החלפת רגש
        // כדי שלא יישאר מצב שבו המשחק חושב שהכפתור עדיין לחוץ
        jumpHeld = false;
        pressedThisFrame = false;
        releasedThisFrame = false;

        currentStrategy?.Enter();
    }

    // 🔥🔥🔥 זה החלק החשוב החדש
    public void OnStaminaDepleted()
    {
        if (isFailing)
            return;

        isFailing = true;

        if (currentStrategy != null)
        {
            currentStrategy.HandleStaminaDepleted();
        }
    }

    public void ResetToNeutral()
    {
        isFailing = false;

        moveInput = Vector2.zero;
        jumpHeld = false;
        pressedThisFrame = false;
        releasedThisFrame = false;

        SetEmotion(EmotionController.Emotion.Neutral);

        if (visualSwitcher != null)
        {
            visualSwitcher.ShowNeutral();
            visualSwitcher.SetDirection(0f);
        }
    }
}