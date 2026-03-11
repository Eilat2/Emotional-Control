using UnityEngine;

public interface IEmotionStrategy
{
    // נקראים כשנכנסים/יוצאים מהרגש
    void Enter();
    void Exit();

    // תנועה (ערך X/Y מה-Input System)
    void HandleMove(Vector2 move);

    /*
     * קפיצה/פעולה (Jump_Break)
     * isHeld            = האם הכפתור מוחזק עכשיו
     * pressedThisFrame  = true רק בפריים שבו הכפתור נלחץ (Down)
     * releasedThisFrame = true רק בפריים שבו הכפתור שוחרר (Up)
     *
     * למה צריך את שלושתם?
     * - pressedThisFrame: כדי לזהות "לחיצה ראשונה/שנייה"
     * - isHeld: כדי לרחף רק בזמן שמחזיקים
     * - releasedThisFrame: כדי להפסיק ריחוף מיד בשחרור
     */
    void HandleJumpBreak(bool isHeld, bool pressedThisFrame, bool releasedThisFrame);

    // נקרא כל פריים מה-Context (Update)
    void Tick();
}
