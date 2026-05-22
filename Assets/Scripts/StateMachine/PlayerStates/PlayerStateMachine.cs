using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("References – גררו מה-Player")]
    [SerializeField] private PlayerEmotionContext emotionContext;
    [SerializeField] private Rigidbody2D rb;

    [Header("Thresholds")]
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float groundedRayDist = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Glide Settings")]
    [SerializeField] private float glideGravityScale = 0.4f;
    public float GlideGravityScale => glideGravityScale;

    [Header("Break Settings")]
    [SerializeField] private float breakLockDuration = 0.3f;
    public float BreakLockDuration => breakLockDuration;

    public Rigidbody2D Rb => rb;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerGlideState GlideState { get; private set; }
    public PlayerRageBreakState RageBreakState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    public IPlayerState CurrentState { get; private set; }
    public string CurrentStateLabel => CurrentState?.GetType().Name ?? "None";

    public bool IsGrounded { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (emotionContext == null)
            emotionContext = GetComponent<PlayerEmotionContext>();

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        JumpState = new PlayerJumpState(this);
        GlideState = new PlayerGlideState(this);
        RageBreakState = new PlayerRageBreakState(this);
        DeadState = new PlayerDeadState(this);
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void Start()
    {
        Debug.Log("[StateMachine] STARTED");
        TransitionTo(IdleState);
    }

    private void Update()
    {
        if (CurrentState == null)
            return;

        RefreshSensors();
        CurrentState.Update();

        if (IsDead)
            return;

        AutoTransition();
    }

    private void RefreshSensors()
    {
        IsGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundedRayDist,
            groundLayer
        );

        IsMoving = rb != null && Mathf.Abs(rb.linearVelocity.x) > moveThreshold;
    }

    private void AutoTransition()
    {
        if (CurrentState == RageBreakState)
            return;

        if (CurrentState == GlideState)
            return;

        if (CurrentState == IdleState)
        {
            if (!IsGrounded)
                TransitionTo(JumpState);
            else if (IsMoving)
                TransitionTo(WalkState);
        }
        else if (CurrentState == WalkState)
        {
            if (!IsGrounded)
                TransitionTo(JumpState);
            else if (!IsMoving)
                TransitionTo(IdleState);
        }
        else if (CurrentState == JumpState)
        {
            if (IsGrounded && IsMoving)
                TransitionTo(WalkState);
            else if (IsGrounded)
                TransitionTo(IdleState);
        }
    }

    public void TransitionTo(IPlayerState nextState)
    {
        if (nextState == null || nextState == CurrentState)
            return;

        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();

        Debug.Log($"[StateMachine] → {CurrentStateLabel}");
    }

    public void TriggerBreak(IBreakable target)
    {
        if (target == null)
            return;

        RageBreakState.SetTarget(target);
        TransitionTo(RageBreakState);
    }

    public void StartGlide()
    {
        TransitionTo(GlideState);
    }

    public void StopGlide()
    {
        if (CurrentState == GlideState)
        {
            TransitionTo(IsGrounded ? IdleState : JumpState);
        }
    }

    private void HandleGameOver()
    {
        IsDead = true;
        TransitionTo(DeadState);
    }

    public void ResetMachine()
    {
        IsDead = false;
        TransitionTo(IdleState);
    }

    public PlayerEmotionContext EmotionContext => emotionContext;
}