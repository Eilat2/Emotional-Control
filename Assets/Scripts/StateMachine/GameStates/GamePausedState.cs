using UnityEngine;

// ============================================================
//  GamePausedState  –  המשחק מושהה
//
//  עוטף את PauseMenuInputSystem הקיים — לא מחליף אותו!
//  PauseMenuInputSystem ממשיך לנהל את ה-UI בפועל.
//  GamePausedState רק "יודע" שאנחנו ב-Pause.
//
//  כניסה:  Playing (ESC)
//  יציאה:  ESC שוב / כפתור Resume → PlayingState
// ============================================================

public class GamePausedState : IGameState
{
    private readonly GameStateMachine _machine;
    private readonly PauseMenuInputSystem _pauseMenu;

    public GamePausedState(GameStateMachine machine, PauseMenuInputSystem pauseMenu)
    {
        _machine = machine;
        _pauseMenu = pauseMenu;
    }

    public void Enter()
    {
        // PauseMenuInputSystem.Pause() כבר:
        //   • מציג PausePanel
        //   • עוצר Time.timeScale = 0
        //   • עוצר תנועת שחקן
        // אנחנו רק קוראים לו — לא כותבים כפול
        _pauseMenu?.Pause();
        Debug.Log("[GameState] Paused");
    }

    public void Update()
    {
        // PauseMenuInputSystem מנהל את ה-ESC בעצמו —
        // לא צריך לנהל כאן.
    }

    public void Exit()
    {
        _pauseMenu?.Resume();
    }
}
