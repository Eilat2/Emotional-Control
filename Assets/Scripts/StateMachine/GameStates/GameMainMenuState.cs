using UnityEngine;

// ============================================================
//  GameMainMenuState – תפריט ראשי
//
//  כרגע Main Menu הוא סצנה נפרדת, אז המצב הזה קיים
//  בעיקר לשלמות המבנה. אם בעתיד יהיה Main Menu באותה
//  סצנה – הכל כאן.
// ============================================================
public class GameMainMenuState : GameStateBase
{
    public GameMainMenuState(GameStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        Time.timeScale = 1f;
        base.Enter();
        // כאן: הצגת Main Menu panel, ניגון מוזיקת תפריט וכו'.
    }

    public override void Exit()
    {
        base.Exit();
        // כאן: הסתרת Main Menu panel.
    }
}
