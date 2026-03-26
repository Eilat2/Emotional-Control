using UnityEngine;

public class EmotionController : MonoBehaviour
{
    // הגדרת סוג נתונים חדש שמייצג את שלושת הרגשות האפשריים של הדמות
    public enum Emotion { Neutral, Joy, Rage }

    // משתנה ששומר איזה רגש פעיל כרגע (ברירת מחדל Neutral)
    public Emotion current = Emotion.Neutral;

    // ---------- סקריפטים של Neutral ----------
    [Header("Neutral scripts")]
    // הסקריפט שמפעיל את התנועה של הדמות במצב Neutral
    [SerializeField] MonoBehaviour neutralMovement;

    // ---------- סקריפטים של Joy ----------
    [Header("Joy scripts")]
    // הסקריפט שמפעיל את התנועה של הדמות במצב Joy
    [SerializeField] MonoBehaviour joyMovement;

    // ---------- סקריפטים של Rage ----------
    [Header("Rage scripts")]
    // הסקריפט של התנועה במצב Rage
    [SerializeField] MonoBehaviour rageMovement;

    // הסקריפט שמאפשר לשבור קירות במצב Rage
    [SerializeField] MonoBehaviour rageBreak;

    // ---------- חלק ויזואלי ----------
    [Header("Visual")]

    // אם עדיין יש SpriteRenderer על ה-Player הראשי
    [SerializeField] SpriteRenderer playerRenderer;

    // הקומפוננטה שמנהלת את הרגש של הדמות במערכת המשחק
    [SerializeField] PlayerEmotionContext context;

    // הסקריפט שמחליף בין שלושת הוויזואלים של הדמות
    [SerializeField] PlayerVisualSwitcher visualSwitcher;

    // צבעים שונים לכל רגש
    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void Start()
    {
        // אם לא חיברנו ידנית SpriteRenderer דרך ה-Inspector,
        // Unity ימצא אותו אוטומטית על האובייקט
        if (!playerRenderer) playerRenderer = GetComponent<SpriteRenderer>();

        // אם לא חיברנו את PlayerEmotionContext,
        // Unity ינסה למצוא אותו על אותו אובייקט
        if (!context) context = GetComponent<PlayerEmotionContext>();

        // אם לא חיברנו את PlayerVisualSwitcher,
        // Unity ינסה למצוא אותו על אותו אובייקט
        if (!visualSwitcher) visualSwitcher = GetComponent<PlayerVisualSwitcher>();

        // מפעיל את הרגש ההתחלתי של הדמות
        ApplyInitial(current);
    }

    // פונקציות שמחליפות את הרגש כאשר קוראים להן (למשל דרך כפתורים או Input)
    public void OnJoy() => Apply(Emotion.Joy);
    public void OnAnger() => Apply(Emotion.Rage);
    public void OnNeutral() => Apply(Emotion.Neutral);

    // פונקציה שמחליפה את הרגש של הדמות בזמן המשחק
    void Apply(Emotion e)
    {
        // אם הדמות כבר ברגש הזה לא עושים שינוי
        if (current == e) return;

        current = e;

        // מעדכן את מערכת הרגשות במשחק
        context?.SetEmotion(e);

        // משנה את הוויזואל של הדמות לפי הרגש החדש
        ApplyVisual(e);
    }

    // הפעלה ראשונית של הרגש כשהמשחק מתחיל
    void ApplyInitial(Emotion e)
    {
        current = e;

        // מעדכן את מערכת הרגשות
        context?.SetEmotion(e);

        // משנה את הוויזואל של הדמות
        ApplyVisual(e);
    }

    // פונקציה שמעדכנת את הוויזואל של הדמות לפי הרגש
    void ApplyVisual(Emotion e)
    {
        // קודם כל מחליפים את הדמות המוצגת
        if (visualSwitcher != null)
        {
            if (e == Emotion.Joy)
                visualSwitcher.ShowJoy();
            else if (e == Emotion.Rage)
                visualSwitcher.ShowRage();
            else
                visualSwitcher.ShowNeutral();
        }

        // אופציונלי: אם עדיין יש SpriteRenderer על ה-Player הראשי,
        // נשנה גם את הצבע שלו
        if (!playerRenderer) return;

        if (e == Emotion.Joy)
            playerRenderer.color = joyColor;
        else if (e == Emotion.Rage)
            playerRenderer.color = rageColor;
        else
            playerRenderer.color = neutralColor;
    }
}