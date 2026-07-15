// ============================================================
//  GameOverState – המשחק נגמר (השחקן מת)
//
//  עוטף את PauseMenuInputSystem.GameOver() הקיים.
//  מגיעים לכאן דרך GameEvents.OnGameOver.
//
//  כניסה:  כל מצב → GameEvents.OnGameOver
//  יציאה:  כפתור Restart → GameEvents.OnRestartRequested
//                        → GameStateMachine.HandleRestart()
//                        → PlayingState
// ============================================================
public class GameOverState : GameStateBase
{
    private readonly PauseMenuInputSystem _pauseMenu;

    public GameOverState(GameStateMachine machine, PauseMenuInputSystem pauseMenu)
        : base(machine)
    {
        _pauseMenu = pauseMenu;
    }

    public override void Enter()
    {
        // PauseMenuInputSystem כבר מאזין ל-GameEvents.OnGameOver בעצמו
        // ומטפל ב-GameOverPanel / timeScale / focus.
        // אנחנו רק מתעדים את המצב.
        base.Enter();
    }

    public override void Exit()
    {
        // ניקוי GameOverPanel נעשה כבר ב-PauseMenuInputSystem.Restart()
        // דרך OnRestartRequested – לא צריך כאן כפול.
        base.Exit();
    }
}
