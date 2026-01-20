using UnityEngine;
using UnityEngine.InputSystem;

public class EmotionController : MonoBehaviour
{
    // שלושת המצבים של השחקן
    public enum Emotion { Neutral, Joy, Rage }

    // המצב הנוכחי – המשחק מתחיל על Neutral
    public Emotion current = Emotion.Neutral;

    // ---------- סקריפטים של ניטרלי ----------
    // תנועה בסיסית שתעבוד רק במצב Neutral
    [Header("Neutral scripts")]
    [SerializeField] MonoBehaviour neutralMovement;

    // ---------- סקריפטים של שמחה ----------
    // הסקריפט הזה יעבוד רק במצב Joy
    [Header("Joy scripts")]
    [SerializeField] MonoBehaviour joyMovement;

    // ---------- סקריפטים של זעם ----------
    // הסקריפטים האלה יעבדו רק במצב Rage
    [Header("Rage scripts")]
    [SerializeField] MonoBehaviour rageMovement;
    [SerializeField] MonoBehaviour rageBreak;

    // ---------- ויזואל ----------
    [Header("Visual")]
    [SerializeField] SpriteRenderer playerRenderer;

    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void Start()
    {
        // אם לא גררנו SpriteRenderer ב-Inspector
        // Unity תחפש אותו לבד על השחקן
        if (!playerRenderer)
            playerRenderer = GetComponent<SpriteRenderer>();

        // מפעיל את המצב ההתחלתי (Neutral) ומדליק/מכבה את הסקריפטים בהתאם
        // חשוב: ApplyInitial כדי שלא ייתקע על if(current==e) בתחילת המשחק
        ApplyInitial(current);
    }

    // ---------- קלט (New Input System) ----------
    // נקרא כשנלחץ המקש של שמחה (Q)
    public void OnJoy() => Apply(Emotion.Joy);

    // נקרא כשנלחץ המקש של זעם (E)
    public void OnAnger() => Apply(Emotion.Rage);

    // נקרא כשנלחץ המקש של Neutral (למשל N)
    public void OnNeutral() => Apply(Emotion.Neutral);

    // ---------- החלפת מצב (בזמן משחק) ----------
    void Apply(Emotion e)
    {
        // אם כבר במצב הזה – לא צריך לעשות כלום
        if (current == e) return;

        current = e;

        // מדליק/מכבה סקריפטים לפי המצב הנבחר
        Set(neutralMovement, e == Emotion.Neutral);
        Set(joyMovement, e == Emotion.Joy);
        Set(rageMovement, e == Emotion.Rage);
        Set(rageBreak, e == Emotion.Rage);

        // שינוי צבע דמות לפי מצב (רק ויזואל)
        ApplyVisual(e);
    }

    // ---------- הפעלה ראשונית בתחילת המשחק ----------
    // כאן אנחנו לא עושים return אפילו אם current == e,
    // כי אנחנו רוצים להבטיח שהסקריפטים מוגדרים נכון מהרגע הראשון.
    void ApplyInitial(Emotion e)
    {
        current = e;

        Set(neutralMovement, e == Emotion.Neutral);
        Set(joyMovement, e == Emotion.Joy);
        Set(rageMovement, e == Emotion.Rage);
        Set(rageBreak, e == Emotion.Rage);

        ApplyVisual(e);
    }

    // שינוי צבע השחקן לפי המצב הנוכחי
    void ApplyVisual(Emotion e)
    {
        if (!playerRenderer) return;

        if (e == Emotion.Joy) playerRenderer.color = joyColor;
        else if (e == Emotion.Rage) playerRenderer.color = rageColor;
        else playerRenderer.color = neutralColor;
    }

    // פונקציה כללית: אם יש סקריפט – מדליקה או מכבה אותו
    void Set(MonoBehaviour script, bool enabled)
    {
        if (script) script.enabled = enabled;
    }
}
