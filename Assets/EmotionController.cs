using UnityEngine;

public class EmotionController : MonoBehaviour
{
    public enum Emotion { Joy, Rage }
    public Emotion current = Emotion.Joy;

    [Header("Joy scripts (enable in Joy)")]
    public MonoBehaviour joyMovement;   // הסקריפט של שמחה (תנועה/קפיצה/רחיפה)

    [Header("Rage scripts (enable in Rage)")]
    public MonoBehaviour rageMovement;  // הסקריפט של תזוזת זעם (A/D)
    public MonoBehaviour rageBreak;     // BreakOnTouch

    [Header("Visual")]
    public SpriteRenderer playerRenderer;
    public Color joyColor = Color.yellow;
    public Color rageColor = Color.red;

    void Start()
    {
        // אם לא חיברת ידנית באינספקטור - ניקח אוטומטית מהאובייקט
        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();

        Apply(current);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Apply(Emotion.Joy);
        if (Input.GetKeyDown(KeyCode.E)) Apply(Emotion.Rage);
    }

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

        // בדיקת מקשים (אפשר למחוק אחרי שזה עובד לך)
        Debug.Log("Switched to: " + e);
    }
}
