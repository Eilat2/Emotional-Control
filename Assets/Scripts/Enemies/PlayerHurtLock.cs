using UnityEngine;

// סקריפט קטן שמנהל:
// 1) נעילת תנועה לזמן קצר אחרי פגיעה (כדי שהנוקבאק לא יידרס)
// 2) אינבינסיבליטי (i-frames) – זמן קצר שאי אפשר לקבל בו עוד פגיעות
public class PlayerHurtLock : MonoBehaviour
{
    float lockUntil = 0f;
    float invincibleUntil = 0f;

    // האם כרגע התנועה נעולה (לא מאפשרים לסקריפטי תנועה לדרוס נוקבאק)
    public bool IsLocked => Time.time < lockUntil;

    // האם כרגע השחקן חסין לפגיעה (לא מורידים סטאמינה/לא נוקבאק)
    public bool IsInvincible => Time.time < invincibleUntil;

    // מפעיל פגיעה: גם נעילת תנועה וגם i-frames
    public void TriggerHit(float lockSeconds, float invincibleSeconds)
    {
        lockUntil = Time.time + lockSeconds;
        invincibleUntil = Time.time + invincibleSeconds;
    }
}
