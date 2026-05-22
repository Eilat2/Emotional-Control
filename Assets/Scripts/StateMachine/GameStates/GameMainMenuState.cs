using UnityEngine;

// ============================================================
//  GameMainMenuState  –  תפריט ראשי
//
//  כרגע Main Menu הוא סצנה נפרדת אצלכן, אז המצב הזה
//  קיים בעיקר לשלמות המבנה ולמצגת.
//  אם בעתיד תרצו Main Menu באותה סצנה – הכל כאן.
// ============================================================

public class GameMainMenuState : IGameState
{
    private readonly GameStateMachine _machine;

    public GameMainMenuState(GameStateMachine machine)
    {
        _machine = machine;
    }

    public void Enter()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameState] Main Menu");
        // כאן: הצגת Main Menu panel, ניגון מוזיקת תפריט וכו'
    }

    public void Update() { }

    public void Exit()
    {
        // כאן: הסתרת Main Menu panel
    }
}
