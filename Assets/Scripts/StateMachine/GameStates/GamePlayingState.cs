using UnityEngine;

// ============================================================
//  GamePlayingState  –  המשחק רץ
//
//  כניסה:  Start() | Restart (OnRestartRequested)
//  יציאה:  ESC       → PausedState
//          Game Over → GameOverState
// ============================================================

public class GamePlayingState : IGameState
{
    private readonly GameStateMachine _machine;

    public GamePlayingState(GameStateMachine machine)
    {
        _machine = machine;
    }

    public void Enter()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameState] Playing");
        // כאן: ניגון מוזיקת gameplay, הצגת HUD וכו'
    }

    public void Update()
    {
        // בדיקת ESC מועברת ל-PauseMenuInputSystem הקיים —
        // הוא כבר עושה את זה. אם רוצים לנהל כאן:
        // if (Keyboard.current.escapeKey.wasPressedThisFrame)
        //     _machine.TransitionTo(_machine.PausedState);
    }

    public void Exit()
    {
        // כאן: עצירת מוזיקת gameplay וכו'
    }
}
