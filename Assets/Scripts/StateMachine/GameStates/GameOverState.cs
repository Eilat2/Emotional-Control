using UnityEngine;

// ============================================================
//  GameOverState  –  המשחק נגמר (השחקן מת)
//
//  עוטף את PauseMenuInputSystem.GameOver() הקיים.
//  מגיעים לכאן דרך GameEvents.OnGameOver.
//
//  כניסה:  כל מצב → GameEvents.OnGameOver
//  יציאה:  כפתור Restart → GameEvents.OnRestartRequested
//                        → GameStateMachine.HandleRestart()
//                        → PlayingState
// ============================================================

public class GameOverState : IGameState
{
    private readonly GameStateMachine _machine;
    private readonly PauseMenuInputSystem _pauseMenu;

    public GameOverState(GameStateMachine machine, PauseMenuInputSystem pauseMenu)
    {
        _machine = machine;
        _pauseMenu = pauseMenu;
    }

    public void Enter()
    {
        // PauseMenuInputSystem.GameOver() כבר:
        //   • מציג GameOverPanel
        //   • עוצר Time.timeScale = 0
        //   • מפנה focus לכפתור Restart
        // אבל — PauseMenuInputSystem מאזין ל-GameEvents.OnGameOver בעצמו,
        // אז הוא כבר נקרא. אנחנו רק מתעדים את המצב.
        Debug.Log("[GameState] Game Over");
    }

    public void Update() { }

    public void Exit()
    {
        // ניקוי GameOverPanel אם צריך —
        // PauseMenuInputSystem.Restart() כבר עושה זאת
        // ב-OnRestartRequested. לא צריך כאן כפול.
        Debug.Log("[GameState] Leaving Game Over");
    }
}
