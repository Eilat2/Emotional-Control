using UnityEngine;

// סקריפט קטן שמנהל:
// 1) נעילת תנועה לזמן קצר אחרי פגיעה (כדי שהנוקבאק לא יידרס)
// 2) אינבינסיבליטי (i-frames) – זמן קצר שאי אפשר לקבל בו עוד פגיעות
public class PlayerHurtLock : MonoBehaviour
{
    private float _lockUntil;
    private float _invincibleUntil;

    // האם כרגע התנועה נעולה (לא מאפשרים לסקריפטי תנועה לדרוס נוקבאק)
    public bool IsLocked => Time.time < _lockUntil;

    // האם כרגע השחקן חסין לפגיעה (לא מורידים סטאמינה/לא נוקבאק)
    public bool IsInvincible => Time.time < _invincibleUntil;

    // מפעיל פגיעה: גם נעילת תנועה וגם i-frames
    public void TriggerHit(float lockSeconds, float invincibleSeconds)
    {
        _lockUntil = Time.time + lockSeconds;
        _invincibleUntil = Time.time + invincibleSeconds;
    }
}
