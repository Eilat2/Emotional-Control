// ============================================================
//  PlayerStateBase – מחלקת אב לכל מצבי השחקן
//
//  מרכזת את הדפוס החוזר בכל state:
//    - שמירת רפרנס ל-machine (Constructor)
//    - לוג סטנדרטי ב-Enter/Exit
//
//  מצב ספציפי יכול לדרוס Enter/Update/Exit ולהוסיף
//  לוגיקה משלו – פשוט לקרוא ל-base.Enter()/base.Exit()
//  כדי לשמור על הלוג האחיד (לא חובה).
// ============================================================
public abstract class PlayerStateBase : IPlayerState
{
    protected readonly PlayerStateMachine Machine;

    protected PlayerStateBase(PlayerStateMachine machine)
    {
        Machine = machine;
    }

    public virtual void Enter() => StateLogger.Log(GetType().Name, "Enter");
    public virtual void Update() { }
    public virtual void Exit() => StateLogger.Log(GetType().Name, "Exit");
}
