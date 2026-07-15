using UnityEngine;

public class EmotionController : MonoBehaviour
{
    public enum Emotion { Neutral, Joy, Rage }

    public Emotion current = Emotion.Neutral;

    [Header("Neutral scripts")]
    [SerializeField] private MonoBehaviour neutralMovement;

    [Header("Joy scripts")]
    [SerializeField] private MonoBehaviour joyMovement;

    [Header("Rage scripts")]
    [SerializeField] private MonoBehaviour rageMovement;
    [SerializeField] private MonoBehaviour rageBreak;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private PlayerEmotionContext context;

    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    private void Awake()
    {
        if (!playerRenderer)
            playerRenderer = GetComponent<SpriteRenderer>();

        if (!context)
            context = GetComponent<PlayerEmotionContext>();
    }

    private void Start()
    {
        // בכל סצנה השחקן מתחיל מחדש כ-Neutral
        // כי לא משתמשים ב-DontDestroyOnLoad.
        ApplyInitial(Emotion.Neutral);
    }

    public void OnJoy() => Apply(Emotion.Joy);
    public void OnAnger() => Apply(Emotion.Rage);
    public void OnNeutral() => Apply(Emotion.Neutral);

    private void Apply(Emotion e)
    {
        if (current == e)
            return;

        current = e;

        context?.SetEmotion(e);
        ApplyColor(e);

        // EmotionController רק מודיע שהרגש השתנה -
        // הוא לא קורא ישירות ל-WorldSwitcher או ל-VisualSwitcher.
        GameEvents.RaiseEmotionChanged(e);
    }

    private void ApplyInitial(Emotion e)
    {
        current = e;

        context?.SetEmotion(e);
        ApplyColor(e);

        GameEvents.RaiseEmotionChanged(e);
    }

    public void ResetToNeutral()
    {
        ApplyInitial(Emotion.Neutral);
    }

    private void ApplyColor(Emotion e)
    {
        if (!playerRenderer)
            return;

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
