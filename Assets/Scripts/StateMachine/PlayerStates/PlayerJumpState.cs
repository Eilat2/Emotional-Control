// ============================================================
//  PlayerJumpState – השחקן באוויר (קפיצה / נפילה)
//
//  כניסה:  Idle / Walk (עזב את הקרקע)
//  יציאה:  נחת + זז   → WalkState
//          נחת + עצר  → IdleState
//          מת         → DeadState
//
//  הערה: הקפיצה עצמה מנוהלת ע"י IEmotionStrategy (Joy/Rage).
//        JumpState רק "יודע" שאנחנו באוויר ומנגן
//        את ה-animation / הלוגיקה ה-aerial.
// ============================================================
public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerStateMachine machine) : base(machine) { }

    // מקום טבעי להוסיף בעתיד: coyote time, jump buffer, air control.
}
