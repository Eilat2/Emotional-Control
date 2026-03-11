using UnityEngine;

public class EmotionController : MonoBehaviour
{
    public enum Emotion { Neutral, Joy, Rage }
    public Emotion current = Emotion.Neutral;

    // ---------- �������� �� ������ ----------
    // ����� ������ ������ �� ���� Neutral
    [Header("Neutral scripts")]
    [SerializeField] MonoBehaviour neutralMovement;
    
    // ---------- �������� �� ���� ----------
    // ������� ��� ����� �� ���� Joy
    [Header("Joy scripts")]
    [SerializeField] MonoBehaviour joyMovement;

    // ---------- �������� �� ��� ----------
    // ��������� ���� ����� �� ���� Rage
    [Header("Rage scripts")]
    [SerializeField] MonoBehaviour rageMovement;
    [SerializeField] MonoBehaviour rageBreak;

    // ---------- ������ ----------
    [Header("Visual")]
    [SerializeField] SpriteRenderer playerRenderer;
    public Color neutralColor = Color.white;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void Start()
    {
        if (!playerRenderer) playerRenderer = GetComponent<SpriteRenderer>();
        if (!context) context = GetComponent<PlayerEmotionContext>();

        ApplyInitial(current);
    }

    public void OnJoy() => Apply(Emotion.Joy);
    public void OnAnger() => Apply(Emotion.Rage);
    public void OnNeutral() => Apply(Emotion.Neutral);

    void Apply(Emotion e)
    {
        if (current == e) return;
        current = e;

        context?.SetEmotion(e);
        ApplyVisual(e);
    }

    void ApplyInitial(Emotion e)
    {
        current = e;
        context?.SetEmotion(e);
        ApplyVisual(e);
    }

    void ApplyVisual(Emotion e)
    {
        if (!playerRenderer) return;

        if (e == Emotion.Joy) playerRenderer.color = joyColor;
        else if (e == Emotion.Rage) playerRenderer.color = rageColor;
        else playerRenderer.color = neutralColor;
    }
}
