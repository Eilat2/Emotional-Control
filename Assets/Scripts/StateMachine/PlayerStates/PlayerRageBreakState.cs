using UnityEngine;

// ============================================================
//  PlayerRageBreakState – Rage שוברת מטרה (IBreakable)
//
//  כניסה:  נקבעת מטרה (SetTarget) ואז TriggerBreak על ה-machine
//  יציאה:  אחרי BreakLockDuration שניות → IdleState
// ============================================================
public class PlayerRageBreakState : PlayerStateBase
{
    private IBreakable _target;
    private float _exitTime;

    public PlayerRageBreakState(PlayerStateMachine machine) : base(machine) { }

    public void SetTarget(IBreakable target)
    {
        _target = target;
    }

    public override void Enter()
    {
        base.Enter();

        if (_target != null)
        {
            _target.OnBreak();
            StateLogger.Log(nameof(PlayerRageBreakState), $"Broke: {_target}");
        }
        else
        {
            StateLogger.Warn(nameof(PlayerRageBreakState), "No target to break!");
        }

        _exitTime = Time.time + Machine.BreakLockDuration;

        Rigidbody2D rb = Machine.Rb;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        if (Time.time >= _exitTime)
            Machine.TransitionTo(Machine.IdleState);
    }

    public override void Exit()
    {
        // איפוס המטרה מתבצע פעם אחת, כאן בלבד
        // (לפני התיקון זה קרה גם ב-Update וגם ב-Exit).
        _target = null;
        base.Exit();
    }
}
