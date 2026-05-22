using UnityEngine;

// ============================================================
//  PlayerGlideState  –  Joy גולשת באוויר
//
//  כניסה:  JumpState → כשהרגש הופך Joy ו-jumpHeld == true
//          (קוראים ל-TransitionTo מבחוץ מה-JoyStrategy)
//
//  יציאה:  נחתה        → Run / Idle
//          שחררה כפתור → JumpState (נפילה רגילה)
//          מתה         → DeadState
//
//  חיבור קיים:
//    JoyStrategy אמור לקרוא ל:
//      machine.TransitionTo(machine.GlideState)  – בכניסה לגלישה
//      machine.TransitionTo(machine.JumpState)   – ביציאה מגלישה
// ============================================================

public class PlayerGlideState : IPlayerState
{
    private readonly PlayerStateMachine _machine;

    // ─── כבידה מוחלשת בגלישה ───────────────────────────────
    // הערך נשמר ב-Enter ומשוחזר ב-Exit
    private float _originalGravity;

    public PlayerGlideState(PlayerStateMachine machine)
    {
        _machine = machine;
    }

    public void Enter()
    {
        Rigidbody2D rb = _machine.Rb;

        if (rb != null)
        {
            _originalGravity = rb.gravityScale;
            rb.gravityScale = _machine.GlideGravityScale; // ברירת מחדל: 0.4
        }

        // כאן: animation trigger "Glide", particles, glide SFX וכו'
        Debug.Log("[GlideState] Enter – Joy is gliding");
    }

    public void Update()
    {
        // לוגיקת גלישה נוספת אפשר כאן –
        // למשל: cap מהירות אנכית, glide particles וכו'
    }

    public void Exit()
    {
        // מחזירים כבידה רגילה
        Rigidbody2D rb = _machine.Rb;
        if (rb != null)
            rb.gravityScale = _originalGravity;

        Debug.Log("[GlideState] Exit");
    }
}
