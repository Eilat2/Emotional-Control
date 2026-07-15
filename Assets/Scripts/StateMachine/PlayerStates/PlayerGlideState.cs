using UnityEngine;

// ============================================================
//  PlayerGlideState – Joy גולשת באוויר
//
//  כניסה:  JumpState → כשהרגש הופך Joy ו-jumpHeld == true
//          (קוראים ל-TransitionTo מבחוץ מה-JoyStrategy)
//
//  יציאה:  נחתה        → Walk / Idle
//          שחררה כפתור → JumpState (נפילה רגילה)
//          מתה         → DeadState
//
//  חיבור קיים:
//    JoyStrategy אמור לקרוא ל:
//      machine.TransitionTo(machine.GlideState)  – בכניסה לגלישה
//      machine.TransitionTo(machine.JumpState)   – ביציאה מגלישה
// ============================================================
public class PlayerGlideState : PlayerStateBase
{
    // כבידה מוחלשת בגלישה – נשמרת ב-Enter ומשוחזרת ב-Exit
    private float _originalGravity;

    public PlayerGlideState(PlayerStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        base.Enter();

        Rigidbody2D rb = Machine.Rb;
        if (rb != null)
        {
            _originalGravity = rb.gravityScale;
            rb.gravityScale = Machine.GlideGravityScale;
        }

        // כאן: animation trigger "Glide", particles, glide SFX וכו'.
    }

    public override void Exit()
    {
        Rigidbody2D rb = Machine.Rb;
        if (rb != null)
            rb.gravityScale = _originalGravity;

        base.Exit();
    }
}
