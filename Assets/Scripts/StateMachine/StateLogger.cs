using UnityEngine;

// ============================================================
//  StateLogger – עטיפה דקה סביב Debug.Log
//
//  [Conditional("UNITY_EDITOR")] גורם לכל הקריאות האלה
//  להיעלם לגמרי מה-build הסופי (לא רק "לא להדפיס" –
//  הן ממש לא נכנסות לקומפילציה), בלי לצטט/למחוק ידנית
//  את כל שורות ה-Debug.Log שפזורות בכל ה-states.
// ============================================================
public static class StateLogger
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(string source, string message)
    {
        Debug.Log($"[{source}] {message}");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Warn(string source, string message)
    {
        Debug.LogWarning($"[{source}] {message}");
    }
}
