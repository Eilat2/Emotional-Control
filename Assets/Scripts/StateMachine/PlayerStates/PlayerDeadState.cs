using UnityEngine;

public class PlayerDeadState : IPlayerState
{
    private readonly PlayerStateMachine machine;

    public PlayerDeadState(PlayerStateMachine machine)
    {
        this.machine = machine;
    }

    public void Enter()
    {
        Debug.Log("[DeadState] Enter - Game Over");
    }

    public void Update()
    {
        // השחקן מת – אין לוגיקה שוטפת
    }

    public void Exit()
    {
        Debug.Log("[DeadState] Exit - Resetting");
    }
}