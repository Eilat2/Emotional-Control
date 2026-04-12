using System.Collections;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    [Header("Flash Settings")]
    // משך הזמן שהדמות תישאר בצבע פלאש
    [SerializeField] private float flashDuration = 0.12f;

    // ה-SpriteRenderer של הדמות (או של ה-visual הפעיל)
    private SpriteRenderer sr;

    // משתנה שמציין אם כרגע מתבצע פלאש
    private bool isFlashing = false;

    // מאפשר לסקריפטים אחרים לדעת אם יש פלאש פעיל
    public bool IsFlashing => isFlashing;

    void Awake()
    {
        // מחפש את ה-SpriteRenderer על האובייקט או בילדים שלו
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // פונקציה חיצונית להפעלת פלאש
    public void Flash()
    {
        // אם לא נמצא SpriteRenderer – אין מה לעשות
        if (sr == null) return;

        // עוצר כל פלאש קודם (אם קיים)
        StopAllCoroutines();

        // מפעיל פלאש חדש
        StartCoroutine(FlashRoutine());
    }

    // הקורוטינה שמבצעת את ההבהוב
    private IEnumerator FlashRoutine()
    {
        // מסמן שיש פלאש פעיל
        isFlashing = true;

        // שומר את הצבע הנוכחי
        Color lastColor = sr.color;

        // משנה ללבן (אפקט פגיעה)
        sr.color = Color.white;

        // מחכה למשך הפלאש
        yield return new WaitForSeconds(flashDuration);

        // מחזיר לצבע הקודם
        sr.color = lastColor;

        // מסמן שהפלאש נגמר
        isFlashing = false;
    }
}