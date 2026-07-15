// ============================================================
//  PlayerWalkState – השחקן הולך על הקרקע
//
//  כניסה:  Idle (התחיל לזוז) | Jump (נחת בזמן תנועה)
//  יציאה:  עצר    → IdleState
//          באוויר → JumpState
//          מת     → DeadState
// ============================================================
public class PlayerWalkState : PlayerStateBase
{
    public PlayerWalkState(PlayerStateMachine machine) : base(machine) { }

    // מקום טבעי להוסיף בעתיד: footstep particles/SFX וכו'.
}
