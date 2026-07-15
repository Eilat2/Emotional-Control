// ============================================================
//  PlayerIdleState – השחקן עומד, לא זז
//
//  כניסה:  מגיע מ-Walk (עצר) או מ-Jump (נחת ולא זז)
//  יציאה:  השחקן מתחיל לזוז → WalkState
//          השחקן באוויר      → JumpState
//          Game Over          → DeadState
// ============================================================
public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerStateMachine machine) : base(machine) { }

    // Idle לא צריך לוגיקת Update מיוחדת –
    // המעברים האוטומטיים ב-PlayerStateMachine.AutoTransition
    // מטפלים בכל הבדיקות.
}
