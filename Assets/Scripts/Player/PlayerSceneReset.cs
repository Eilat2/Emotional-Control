using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneReset : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private float normalGravityScale = 4f; // כוח כבידה רגיל של השחקן

    private Vector3 originalScale; // שומר את הגודל המקורי של השחקן

    private void Awake()
    {
        // שומרים את הגודל ההתחלתי של השחקן
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // נרשמים לאירוע של טעינת סצנה
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // מבטלים הרשמה כדי למנוע באגים/כפילויות
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔥 מחזירים את השחקן לנקודת ההתחלה של השלב
        GameObject respawn = GameObject.Find("PlayerSpawnPoint");

        if (respawn != null)
        {
            transform.position = respawn.transform.position;
        }
        else
        {
            Debug.LogWarning("PlayerSpawnPoint not found in scene.");
        }

        // 🔄 מחזירים גודל רגיל
        transform.localScale = originalScale;

        // 🎨 מחזירים שקיפות של כל הספרייטים
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        // 🧱 מפעילים מחדש קוליידרים
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D col in colliders)
        {
            col.enabled = true;
        }

        // ⚙️ מאפסים Rigidbody (פיזיקה)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = normalGravityScale;
            rb.simulated = true;
        }

        // 🧠 מאפסים את מערכת הרגשות
        PlayerEmotionContext context = GetComponent<PlayerEmotionContext>();
        if (context != null)
        {
            context.ResetToNeutral();
        }

        // 💡 גם את EmotionController (אם קיים)
        EmotionController emotion = GetComponent<EmotionController>();
        if (emotion != null)
        {
            emotion.current = EmotionController.Emotion.Neutral;
        }

        // 🔋 מאפסים סטאמינה
        Stamina[] staminaComponents = GetComponentsInChildren<Stamina>(true);
        foreach (Stamina stamina in staminaComponents)
        {
            stamina.ResetForNewScene();
        }

        // 🔄 מפעילים מחדש כל הסקריפטים (למקרה שהם כובו)
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }
}