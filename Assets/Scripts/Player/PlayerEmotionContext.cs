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

    private IEmotionStrategy _neutralStrategy;
    private IEmotionStrategy _joyStrategy;
    private IEmotionStrategy _rageStrategy;

    private IEmotionStrategy _currentStrategy;

    private Vector2 _moveInput;

    private bool _jumpHeld;
    private bool _pressedThisFrame;
    private bool _releasedThisFrame;

    // מונע טריגר כפול של HandleStaminaDepleted
    private bool _isFailing;

    // חוסם קלט בפעם אחת, במקום לבדוק Time.timeScale/isFailing
    // בנפרד בכל method (Update / FixedUpdate / OnMove / OnJump_Break).
    private bool IsInputBlocked => Time.timeScale == 0f || _isFailing;

    private void Awake()
    {
        if (visualSwitcher == null)
            visualSwitcher = GetComponentInChildren<PlayerVisualSwitcher>();

        _neutralStrategy = neutralStrategyBehaviour as IEmotionStrategy;
        _joyStrategy = joyStrategyBehaviour as IEmotionStrategy;
        _rageStrategy = rageStrategyBehaviour as IEmotionStrategy;

        if (_neutralStrategy == null || _joyStrategy == null || _rageStrategy == null)
            Debug.LogError("PlayerEmotionContext: אחד ה-Strategies לא מממש IEmotionStrategy או לא שוייך באינספקטור.");

        if (visualSwitcher == null)
            Debug.LogWarning("PlayerEmotionContext: לא נמצא PlayerVisualSwitcher.");
    }

    private void Start()
    {
        // בכל טעינת סצנה מתחילים מחדש במצב Neutral
        // כי לכל סצנה יש Player חדש.
        ResetToNeutral();
    }

    private void Update()
    {
        if (IsInputBlocked || _currentStrategy == null)
            return;

        _currentStrategy.HandleMove(_moveInput);
        visualSwitcher?.SetDirection(_moveInput.x);

        _currentStrategy.HandleJumpBreak(_jumpHeld, _pressedThisFrame, _releasedThisFrame);

        _pressedThisFrame = false;
        _releasedThisFrame = false;
    }

    private void FixedUpdate()
    {
        if (IsInputBlocked || _currentStrategy == null)
            return;

        _currentStrategy.Tick();
    }

    public void OnMove(InputValue value)
    {
        if (IsInputBlocked)
        {
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = value.Get<Vector2>();
    }

    public void OnJump_Break(InputValue value)
    {
        if (IsInputBlocked)
            return;

        bool pressed = value.isPressed;

        if (pressed && !_jumpHeld)
            _pressedThisFrame = true;

        if (!pressed && _jumpHeld)
            _releasedThisFrame = true;

        _jumpHeld = pressed;
    }

    public void SetEmotion(EmotionController.Emotion e)
    {
        IEmotionStrategy next =
            e == EmotionController.Emotion.Joy ? _joyStrategy :
            e == EmotionController.Emotion.Rage ? _rageStrategy :
                                                   _neutralStrategy;

        if (next == null)
        {
            Debug.LogError("PlayerEmotionContext: next strategy יצא null.");
            return;
        }

        if (next == _currentStrategy)
            return;

        _currentStrategy?.Exit();
        _currentStrategy = next;

        // מאפסים את מצב הקפיצה בכל החלפת רגש
        // כדי שלא יישאר מצב שבו המשחק חושב שהכפתור עדיין לחוץ.
        _jumpHeld = false;
        _pressedThisFrame = false;
        _releasedThisFrame = false;

        _currentStrategy?.Enter();
    }

    public void OnStaminaDepleted()
    {
        if (_isFailing)
            return;

        _isFailing = true;
        _currentStrategy?.HandleStaminaDepleted();
    }

    public void ResetToNeutral()
    {
        _isFailing = false;

        _moveInput = Vector2.zero;
        _jumpHeld = false;
        _pressedThisFrame = false;
        _releasedThisFrame = false;

        SetEmotion(EmotionController.Emotion.Neutral);

        if (visualSwitcher != null)
        {
            visualSwitcher.ShowNeutral();
            visualSwitcher.SetDirection(0f);
        }
    }
}
