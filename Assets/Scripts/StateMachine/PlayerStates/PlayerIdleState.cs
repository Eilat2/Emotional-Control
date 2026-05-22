using UnityEngine;

// ============================================================
//  PlayerIdleState  –  השחקן עומד, לא זז
//
//  כניסה:  מגיע מ-Run (עצר) או מ-Jump (נחת ולא זז)
//  יציאה:  המשחקן מתחיל לזוז → RunState
//          המשחקן באוויר      → JumpState
//          Game Over          → DeadState
// ============================================================

public class PlayerIdleState : IPlayerState
{
    private readonly PlayerStateMachine _machine;

    public PlayerIdleState(PlayerStateMachine machine)
    {
        _machine = machine;
    }

    public void Enter()
    {
        // כאן אפשר לשים: animation trigger, sound, particles וכו'
        Debug.Log("[IdleState] Enter");
    }

    public void Update()
    {
        // Idle לא צריך לוגיקה מיוחדת –
        // המעברים האוטומטיים ב-PlayerStateMachine.AutoTransition
        // יטפלו בכל הבדיקות.
    }

    public void Exit()
    {
        Debug.Log("[IdleState] Exit");
    }
}
