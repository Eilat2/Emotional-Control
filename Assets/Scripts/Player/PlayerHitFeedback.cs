using System.Collections;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Renderers")]
    // ה־SpriteRenderers לכל מצב רגש
    [SerializeField] private SpriteRenderer neutralRenderer;
    [SerializeField] private SpriteRenderer joyRenderer;
    [SerializeField] private SpriteRenderer rageRenderer;

    [Header("Visuals")]
    // משך כל שלב של הבהוב (כיבוי/הדלקה)
    [SerializeField] private float blinkStepDuration = 0.06f;

    // כמה פעמים להבהב
    [SerializeField] private int blinkCount = 2;

    [Header("References")]
    // רפרנס לסקריפט שמנהל את הרגש הפעיל
    [SerializeField] private EmotionController emotionController;

    // ה־Transforms של הוויזואלים (לא באמת חייבים עכשיו אבל נשאיר)
    [SerializeField] private Transform neutralVisual;
    [SerializeField] private Transform joyVisual;
    [SerializeField] private Transform rageVisual;

    // פונקציה חיצונית להפעלת אפקט פגיעה
    public void PlayHitFeedback()
    {
        StopAllCoroutines(); // עוצר אפקט קודם אם היה
        StartCoroutine(HitRoutine());
    }

    // הקורוטינה שמבצעת את ההבהוב
    private IEnumerator HitRoutine()
    {
        // לוקחים את ה־Renderer של המצב הפעיל
        SpriteRenderer activeRenderer = GetActiveRenderer();

        // אם חסר משהו – יוצאים
        if (activeRenderer == null || emotionController == null)
            yield break;

        // מבצעים הבהוב
        for (int i = 0; i < blinkCount; i++)
        {
            // מכבים את הספרייט (נראה כמו פגיעה)
            activeRenderer.enabled = false;
            yield return new WaitForSeconds(blinkStepDuration);

            // מדליקים חזרה
            activeRenderer.enabled = true;
            yield return new WaitForSeconds(blinkStepDuration);
        }

        // לוודא שבסוף תמיד נשאר דלוק
        activeRenderer.enabled = true;
    }

    // מחזיר את ה־Renderer לפי הרגש הפעיל
    private SpriteRenderer GetActiveRenderer()
    {
        switch (emotionController.current)
        {
            case EmotionController.Emotion.Joy:
                return joyRenderer;

            case EmotionController.Emotion.Rage:
                return rageRenderer;

            default:
                return neutralRenderer;
        }
    }
}