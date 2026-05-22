using UnityEngine;

public class PlayerRageBreakState : IPlayerState
{
    private readonly PlayerStateMachine _machine;

    private IBreakable _target;
    private float _exitTime;

    public PlayerRageBreakState(PlayerStateMachine machine)
    {
        _machine = machine;
    }

    public void SetTarget(IBreakable target)
    {
        _target = target;
    }

    public void Enter()
    {
        if (_target != null)
        {
            _target.OnBreak();
            Debug.Log($"[RageBreakState] Broke: {_target}");
        }
        else
        {
            Debug.LogWarning("[RageBreakState] No target to break!");
        }

        _exitTime = Time.time + _machine.BreakLockDuration;

        Rigidbody2D rb = _machine.Rb;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("[RageBreakState] Enter - Rage breaking");
    }

    public void Update()
    {
        if (Time.time >= _exitTime)
        {
            _target = null;
            _machine.TransitionTo(_machine.IdleState);
        }
    }

    public void Exit()
    {
        _target = null;
        Debug.Log("[RageBreakState] Exit");
    }
}