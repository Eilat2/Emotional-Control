// ============================================================
//  GamePausedState – המשחק מושהה
//
//  עוטף את PauseMenuInputSystem הקיים – לא מחליף אותו!
//  PauseMenuInputSystem ממשיך לנהל את ה-UI בפועל.
//  GamePausedState רק "יודע" שאנחנו ב-Pause.
//
//  כניסה:  Playing (ESC)
//  יציאה:  ESC שוב / כפתור Resume → PlayingState
// ============================================================
public class GamePausedState : GameStateBase
{
    private readonly PauseMenuInputSystem _pauseMenu;

    public GamePausedState(GameStateMachine machine, PauseMenuInputSystem pauseMenu)
        : base(machine)
    {
        _pauseMenu = pauseMenu;
    }

    public override void Enter()
    {
        // PauseMenuInputSystem.Pause() כבר: מציג PausePanel,
        // עוצר Time.timeScale = 0, עוצר תנועת שחקן.
        // אנחנו רק קוראים לו – לא כותבים כפול.
        _pauseMenu?.Pause();
        base.Enter();
    }

    public override void Exit()
    {
        _pauseMenu?.Resume();
        base.Exit();
    }
}
