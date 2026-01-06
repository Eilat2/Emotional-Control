using UnityEngine;
using UnityEngine.InputSystem;

public class EmotionController : MonoBehaviour
{
    public enum Emotion { Joy, Rage }
    public Emotion current = Emotion.Joy;

    [Header("Joy scripts (enable in Joy)")]
    public MonoBehaviour joyMovement;   // הסקריפט של שמחה (תנועה/קפיצה/רחיפה)

    [Header("Rage scripts (enable in Rage)")]
    public MonoBehaviour rageMovement;  // הסקריפט של תזוזת זעם
    public MonoBehaviour rageBreak;     // BreakOnTouch

    [Header("Visual")]
    public SpriteRenderer playerRenderer;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void Start()
    {
        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();

        Apply(current);
    }

    // ---------- New Input System ----------

    // Action בשם "Joy"
    public void OnJoy()
    {
        Apply(Emotion.Joy);
    }

    // Action בשם "Anger"
    public void OnAnger()
    {
        Apply(Emotion.Rage);
    }

    // -------------------------------------

    void Apply(Emotion e)
    {
        current = e;

        if (joyMovement != null)
            joyMovement.enabled = (e == Emotion.Joy);

        if (rageMovement != null)
            rageMovement.enabled = (e == Emotion.Rage);

        if (rageBreak != null)
            rageBreak.enabled = (e == Emotion.Rage);

        // שינוי צבע כדי לראות בעין
        if (playerRenderer != null)
            playerRenderer.color = (e == Emotion.Joy) ? joyColor : rageColor;

        Debug.Log("Switched to: " + e);
    }
}
