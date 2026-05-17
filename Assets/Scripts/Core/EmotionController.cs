using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Emotion World - Level Specific")]
    // הסקריפט שמדליק/מכבה אובייקטים בעולם לפי הרגש.
    // הוא קיים רק בשלבים מסוימים, למשל Level 3.
    // לכן לא חובה לחבר אותו ידנית ב-Inspector.
    [SerializeField] private EmotionWorldSwitcher emotionWorldSwitcher;

    [Header("Visual")]
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] PlayerEmotionContext context;
    [SerializeField] PlayerVisualSwitcher visualSwitcher;

    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void OnEnable()
    {
        // מאזינים לטעינת סצנה חדשה.
        // חשוב במיוחד כי השחקן Persistent ועובר בין סצנות.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // מפסיקים להאזין כדי למנוע כפילויות או בעיות בזיכרון.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (!playerRenderer) playerRenderer = GetComponent<SpriteRenderer>();
        if (!context) context = GetComponent<PlayerEmotionContext>();
        if (!visualSwitcher) visualSwitcher = GetComponent<PlayerVisualSwitcher>();

        // מנסה למצוא EmotionWorldSwitcher בסצנה הנוכחית.
        // אם אין כזה בסצנה, זה פשוט יישאר null ולא יקרה כלום.
        FindEmotionWorldSwitcher();

        ApplyInitial(current);
    }

    // נקרא אוטומטית בכל פעם שסצנה חדשה נטענת
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // מחפשים מחדש את ה-Switcher כי הוא שייך לסצנה החדשה
        FindEmotionWorldSwitcher();

        // מיישמים מחדש את מצב העולם לפי הרגש הנוכחי
        ApplyWorld(current);
    }

    void FindEmotionWorldSwitcher()
    {
        emotionWorldSwitcher = FindFirstObjectByType<EmotionWorldSwitcher>();
    }

    public void OnJoy() => Apply(Emotion.Joy);
    public void OnAnger() => Apply(Emotion.Rage);
    public void OnNeutral() => Apply(Emotion.Neutral);

    void Apply(Emotion e)
    {
        if (current == e) return;

        current = e;

        context?.SetEmotion(e);
        ApplyWorld(e);
        ApplyVisual(e);
    }

    void ApplyInitial(Emotion e)
    {
        current = e;

        context?.SetEmotion(e);
        ApplyWorld(e);
        ApplyVisual(e);
    }

    void ApplyWorld(Emotion e)
    {
        // אם בסצנה אין EmotionWorldSwitcher, לא עושים כלום.
        // ככה שלבים אחרים לא נפגעים.
        if (emotionWorldSwitcher == null) return;

        if (e == Emotion.Joy)
            emotionWorldSwitcher.ShowJoyWorld();
        else if (e == Emotion.Rage)
            emotionWorldSwitcher.ShowRageWorld();
        else
            emotionWorldSwitcher.ShowNeutralWorld();
    }

    void ApplyVisual(Emotion e)
    {
        if (visualSwitcher != null)
        {
            if (e == Emotion.Joy)
                visualSwitcher.ShowJoy();
            else if (e == Emotion.Rage)
                visualSwitcher.ShowRage();
            else
                visualSwitcher.ShowNeutral();
        }

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