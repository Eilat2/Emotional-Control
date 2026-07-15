using UnityEngine;

// ============================================================
//  GamePlayingState – המשחק רץ
//
//  כניסה:  Start() | Restart (OnRestartRequested)
//  יציאה:  ESC       → PausedState
//          Game Over → GameOverState
// ============================================================
public class GamePlayingState : GameStateBase
{
    public GamePlayingState(GameStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        Time.timeScale = 1f;
        base.Enter();
        // כאן: ניגון מוזיקת gameplay, הצגת HUD וכו'.
    }

    // בדיקת ESC מנוהלת ע"י PauseMenuInputSystem הקיים.

    public override void Exit()
    {
        base.Exit();
        // כאן: עצירת מוזיקת gameplay וכו'.
    }
}
