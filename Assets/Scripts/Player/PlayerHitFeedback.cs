using System.Collections;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Renderers")]
    // ה-SpriteRenderers לכל מצב רגש
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

    // הערה: השדות neutralVisual/joyVisual/rageVisual (Transform) הוסרו –
    // הם לא היו בשימוש בפועל בקוד (רק הוגדרו ולא נקראו לעולם).
    // אם בעתיד תרצי לאנימציה/תזוזה על פגיעה, אפשר להוסיף אותם בחזרה.

    public void PlayHitFeedback()
    {
        StopAllCoroutines(); // עוצר אפקט קודם אם היה
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        SpriteRenderer activeRenderer = GetActiveRenderer();

        if (activeRenderer == null || emotionController == null)
            yield break;

        for (int i = 0; i < blinkCount; i++)
        {
            activeRenderer.enabled = false;
            yield return new WaitForSeconds(blinkStepDuration);

            activeRenderer.enabled = true;
            yield return new WaitForSeconds(blinkStepDuration);
        }

        // לוודא שבסוף תמיד נשאר דלוק
        activeRenderer.enabled = true;
    }

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
