using UnityEngine;

// ============================================================
//  PlayerWalkState  –  השחקן הולך על הקרקע
//
//  כניסה:  Idle (התחיל לזוז) | Jump (נחת בזמן תנועה)
//  יציאה:  עצר    → IdleState
//          באוויר → JumpState
//          מת     → DeadState
// ============================================================

public class PlayerWalkState : IPlayerState
{
    private readonly PlayerStateMachine _machine;

    public PlayerWalkState(PlayerStateMachine machine)
    {
        _machine = machine;
    }

    public void Enter()
    {
        // טריגר ל-Walk animation, Walk SFX וכו'
        Debug.Log("[WalkState] Enter");
    }

    public void Update()
    {
        // לוגיקת הליכה מיוחדת אפשר להוסיף כאן –
        // למשל: particles, footsteps וכו'
    }

    public void Exit()
    {
        Debug.Log("[WalkState] Exit");
    }
}