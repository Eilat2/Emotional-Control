using UnityEngine;
using UnityEngine.InputSystem;

public class EmotionController : MonoBehaviour
{

    public enum Emotion { Neutral, Joy, Rage }

    // המצב הנוכחי – המשחק מתחיל על Neutral
    public Emotion current = Emotion.Neutral;

    // ---------- סקריפטים של שמחה ----------
    // הסקריפט הזה יעבוד רק במצב Joy
    [Header("Joy scripts")]
    public MonoBehaviour joyMovement;

    // ---------- סקריפטים של זעם ----------
    // הסקריפטים האלה יעבדו רק במצב Rage
    [Header("Rage scripts")]
    public MonoBehaviour rageMovement;
    public MonoBehaviour rageBreak;

    // ---------- ויזואל ----------
    [Header("Visual")]
    public SpriteRenderer playerRenderer;

    
    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void Start()
    {
        // אם לא גררנו SpriteRenderer ב-Inspector
        // Unity תחפש אותו לבד על השחקן
        if (!playerRenderer)
            playerRenderer = GetComponent<SpriteRenderer>();

        // מפעיל את המצב ההתחלתי (Neutral)
        Apply(current);
    }

    // ---------- קלט (New Input System) ----------
    // נקרא כשנלחץ המקש של שמחה (Q)
    public void OnJoy() => Apply(Emotion.Joy);

    // נקרא כשנלחץ המקש של זעם (E)
    public void OnAnger() => Apply(Emotion.Rage);

    // נקרא כשנלחץ המקש של Neutral (למשל N)
    public void OnNeutral() => Apply(Emotion.Neutral);

    // ---------- החלפת מצב ----------
    void Apply(Emotion e)
    {
        
        if (current == e) return;
        current = e;

        // מדליק / מכבה סקריפטים לפי המצב
        // Neutral מכבה את כולם
        Set(joyMovement, e == Emotion.Joy);
        Set(rageMovement, e == Emotion.Rage);
        Set(rageBreak, e == Emotion.Rage);

        
        if (playerRenderer)
        {
            if (e == Emotion.Joy) playerRenderer.color = joyColor;
            else if (e == Emotion.Rage) playerRenderer.color = rageColor;
            else playerRenderer.color = neutralColor;
        }
    }

    // אם יש סקריפט – מדליקה או מכבה אותו
    void Set(MonoBehaviour script, bool enabled)
    {
        if (script) script.enabled = enabled;
    }
}
