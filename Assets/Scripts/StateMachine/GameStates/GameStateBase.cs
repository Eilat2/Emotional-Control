// ============================================================
//  GameStateBase – מחלקת אב לכל מצבי המשחק (מקבילה ל-PlayerStateBase)
// ============================================================
public abstract class GameStateBase : IGameState
{
    protected readonly GameStateMachine Machine;

    protected GameStateBase(GameStateMachine machine)
    {
        Machine = machine;
    }

    public virtual void Enter() => StateLogger.Log(GetType().Name, "Enter");
    public virtual void Update() { }
    public virtual void Exit() => StateLogger.Log(GetType().Name, "Exit");
}
