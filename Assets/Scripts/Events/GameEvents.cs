using System;

// ============================================================
//  GameEvents – "אוטובוס" האירועים המרכזי של המשחק.
//
//  כל מקום בקוד שרוצה להודיע "קרה משהו" (רגש השתנה, סטאמינה
//  השתנתה, Game Over, בקשת ריסטארט, אויב מת) קורא ל-Raise... כאן,
//  וכל מי שמתעניין נרשם ל-On... המתאים (בד"כ ב-OnEnable/OnDisable).
// ============================================================
public static class GameEvents
{
    public static event Action<EmotionController.Emotion> OnEmotionChanged;
    public static event Action OnGameOver;
    public static event Action OnRestartRequested;

    public static event Action<Stamina.StaminaType, float, float> OnStaminaChanged;

    // נוסף: מודיע שאויב מת, בלי שה-KillableEnemy צריך להכיר
    // או לחפש את EnemyLevelCounter (לפני זה זה היה FindFirstObjectByType
    // בכל מוות אויב - חיפוש מלא בסצנה, שוב ושוב).
    public static event Action OnEnemyDied;

    public static void RaiseEmotionChanged(EmotionController.Emotion emotion)
    {
        OnEmotionChanged?.Invoke(emotion);
    }

    public static void RaiseGameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void RaiseRestartRequested()
    {
        OnRestartRequested?.Invoke();
    }

    public static void RaiseStaminaChanged(Stamina.StaminaType type, float current, float max)
    {
        OnStaminaChanged?.Invoke(type, current, max);
    }

    public static void RaiseEnemyDied()
    {
        OnEnemyDied?.Invoke();
    }
}
