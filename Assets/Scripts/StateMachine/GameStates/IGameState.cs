// ============================================================
//  IGameState  –  ממשק בסיסי לכל מצב של המשחק
//
//  זהה בעיקרון ל-IPlayerState, אבל ברמת המשחק הכולל.
//  כל מצב (MainMenu, Playing, Paused, GameOver) מממש ממשק זה.
// ============================================================

public interface IGameState
{
    /// <summary>נקרא פעם אחת בכניסה למצב</summary>
    void Enter();

    /// <summary>נקרא כל פריים</summary>
    void Update();

    /// <summary>נקרא פעם אחת ביציאה מהמצב</summary>
    void Exit();
}
