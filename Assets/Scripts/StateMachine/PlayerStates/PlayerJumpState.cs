using UnityEngine;

// ============================================================
//  PlayerJumpState  –  השחקן באוויר (קפיצה / נפילה)
//
//  כניסה:  Idle / Run (עזב את הקרקע)
//  יציאה:  נחת + זז   → RunState
//          נחת + עצר  → IdleState
//          מת         → DeadState
//
//  הערה: הקפיצה עצמה מנוהלת ע"י IEmotionStrategy (Joy/Rage).
//        JumpState רק "יודע" שאנחנו באוויר ומנגן
//        את ה-animation / הלוגיקה ה-aerial.
// ============================================================

public class PlayerJumpState : IPlayerState
{
    private readonly PlayerStateMachine _machine;

    public PlayerJumpState(PlayerStateMachine machine)
    {
        _machine = machine;
    }

    public void Enter()
    {
        // טריגר ל-Jump animation, jump SFX וכו'
        Debug.Log("[JumpState] Enter");
    }

    public void Update()
    {
        // לוגיקה אווירית אפשר להוסיף כאן –
        // למשל: coyote time, jump buffer, air control מיוחד
    }

    public void Exit()
    {
        Debug.Log("[JumpState] Exit");
    }
}
