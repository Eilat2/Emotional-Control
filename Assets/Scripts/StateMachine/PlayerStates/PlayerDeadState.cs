// ============================================================
//  PlayerDeadState – Game Over
// ============================================================
public class PlayerDeadState : PlayerStateBase
{
    public PlayerDeadState(PlayerStateMachine machine) : base(machine) { }

    // השחקן מת – אין לוגיקת Update שוטפת.
}
