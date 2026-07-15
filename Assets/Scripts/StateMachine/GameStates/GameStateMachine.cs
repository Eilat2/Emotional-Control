using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PauseMenuInputSystem pauseMenu;

    public GameMainMenuState MainMenuState { get; private set; }
    public GamePlayingState PlayingState { get; private set; }
    public GamePausedState PausedState { get; private set; }
    public GameOverState GameOverState { get; private set; }

    public IGameState CurrentState { get; private set; }
    public string CurrentStateLabel => CurrentState?.GetType().Name ?? "None";

    public PauseMenuInputSystem PauseMenu => pauseMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (pauseMenu == null)
            pauseMenu = FindFirstObjectByType<PauseMenuInputSystem>();

        MainMenuState = new GameMainMenuState(this);
        PlayingState = new GamePlayingState(this);
        PausedState = new GamePausedState(this, pauseMenu);
        GameOverState = new GameOverState(this, pauseMenu);
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnRestartRequested += HandleRestart;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= HandleGameOver;
        GameEvents.OnRestartRequested -= HandleRestart;
    }

    private void Start()
    {
        TransitionTo(PlayingState);
    }

    private void Update()
    {
        CurrentState?.Update();
    }

    public void TransitionTo(IGameState nextState)
    {
        if (nextState == null || nextState == CurrentState)
            return;

        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();

        StateLogger.Log(nameof(GameStateMachine), $"-> {CurrentStateLabel}");
    }

    private void HandleGameOver()
    {
        TransitionTo(GameOverState);
    }

    private void HandleRestart()
    {
        TransitionTo(PlayingState);
    }
}
