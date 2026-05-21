using UnityEngine;

public class EmotionController : MonoBehaviour
{
    public enum Emotion { Neutral, Joy, Rage }

    public Emotion current = Emotion.Neutral;

    [Header("Neutral scripts")]
    [SerializeField] MonoBehaviour neutralMovement;

    [Header("Joy scripts")]
    [SerializeField] MonoBehaviour joyMovement;

    [Header("Rage scripts")]
    [SerializeField] MonoBehaviour rageMovement;
    [SerializeField] MonoBehaviour rageBreak;

    [Header("Visual")]
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] PlayerEmotionContext context;

    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    // 🔹 מאתחלים רפרנסים אם חסרים
    void Awake()
    {
        if (!playerRenderer)
            playerRenderer = GetComponent<SpriteRenderer>();

        if (!context)
            context = GetComponent<PlayerEmotionContext>();
    }

    void Start()
    {
        // 🔥 בכל סצנה השחקן מתחיל מחדש כ-Normal/Neutral
        // כי כבר לא משתמשים ב-DontDestroyOnLoad
        ApplyInitial(Emotion.Neutral);
    }

    public void OnJoy() => Apply(Emotion.Joy);
    public void OnAnger() => Apply(Emotion.Rage);
    public void OnNeutral() => Apply(Emotion.Neutral);

    void Apply(Emotion e)
    {
        if (current == e) return;

        current = e;

        context?.SetEmotion(e);
        ApplyColor(e);

        // כאן EmotionController רק מודיע שהרגש השתנה.
        // הוא לא קורא ישירות ל-WorldSwitcher או ל-VisualSwitcher.
        GameEvents.RaiseEmotionChanged(e);
    }

    void ApplyInitial(Emotion e)
    {
        current = e;

        context?.SetEmotion(e);
        ApplyColor(e);

        GameEvents.RaiseEmotionChanged(e);
    }

    // 🔹 פונקציה שאפשר לקרוא לה אם רוצים לאפס את השחקן לניטרלי
    public void ResetToNeutral()
    {
        ApplyInitial(Emotion.Neutral);
    }

    void ApplyColor(Emotion e)
    {
        if (!playerRenderer) return;

        if (e == Emotion.Joy)
            playerRenderer.color = joyColor;
        else if (e == Emotion.Rage)
            playerRenderer.color = rageColor;
        else
            playerRenderer.color = neutralColor;
    }

    public EmotionType GetCurrentEmotion()
    {
        switch (current)
        {
            case Emotion.Joy:
                return EmotionType.Joy;

            case Emotion.Rage:
                return EmotionType.Rage;

            default:
                return EmotionType.Neutral;
        }
    }
}