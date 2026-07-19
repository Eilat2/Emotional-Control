using UnityEngine;

// ============================================================
//  PatrolPointCycler – לוגיקת מעבר בין נקודות פטרול, משותפת
//  ל-ArmoredEnemyPatrol (קרקע) ול-FlyingEnemyMovement (אוויר).
//
//  שני הסקריפטים האלה עשו בעבר בדיוק את אותה בדיקה
//  ("הגעתי מספיק קרוב ליעד? -> תעבור ליעד הבא במעגל")
//  כל אחד בנפרד. זה לא MonoBehaviour בכוונה - זה עוזר רגיל
//  (plain C# class) כי אין לו צורך ב-Update/Inspector משלו,
//  רק state קטן ומתודה אחת.
// ============================================================
public class PatrolPointCycler
{
    private readonly Transform[] _points;
    private int _currentIndex;

    public PatrolPointCycler(Transform[] points)
    {
        _points = points;
    }

    public bool HasPoints => _points != null && _points.Length > 0;

    public Transform Current => _points[_currentIndex];

    /// <summary>
    /// בודק אם המרחק שנמדד ע"י הקורא ל-Current הוא בטווח,
    /// ואם כן - מתקדם לנקודה הבאה במעגל ומחזיר אותה.
    ///
    /// המרחק מחושב ע"י הקורא (לא כאן בכוונה!) כי
    /// ArmoredEnemyPatrol (קרקע) בודק מרחק על ציר X בלבד
    /// (כדי לא "לדלג" תור בגלל הפרש גובה), בעוד
    /// FlyingEnemyMovement (אוויר) בודק מרחק דו-מימדי מלא.
    /// זה שימר את ההתנהגות המדויקת של כל אחד מהם.
    /// </summary>
    public Transform AdvanceIfReached(float distanceToCurrent, float reachDistance)
    {
        if (distanceToCurrent <= reachDistance)
            _currentIndex = (_currentIndex + 1) % _points.Length;

        return Current;
    }
}
