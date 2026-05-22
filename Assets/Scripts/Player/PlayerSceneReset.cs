using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
//  PlayerSceneReset  (עדכון)
//
//  שינוי יחיד מהגרסה הקודמת:
//    נוספה שורה אחת בסוף OnSceneLoaded שמאפסת את
//    ה-StateMachine החדשה → ResetMachine()
//
//  שאר הקוד זהה לחלוטין.
// ============================================================

public class PlayerSceneReset : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private float normalGravityScale = 4f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔥 מחזירים לנקודת ספאון
        GameObject respawn = GameObject.Find("PlayerSpawnPoint");
        if (respawn != null)
            transform.position = respawn.transform.position;
        else
            Debug.LogWarning("PlayerSpawnPoint not found in scene.");

        // 🔄 גודל רגיל
        transform.localScale = originalScale;

        // 🎨 שקיפות ספרייטים
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        // 🧱 קוליידרים
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D col in colliders)
            col.enabled = true;

        // ⚙️ פיזיקה
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = normalGravityScale;
            rb.simulated = true;
        }

        // 🧠 מערכת רגשות
        PlayerEmotionContext context = GetComponent<PlayerEmotionContext>();
        if (context != null)
            context.ResetToNeutral();

        // 💡 EmotionController
        EmotionController emotion = GetComponent<EmotionController>();
        if (emotion != null)
        {
            emotion.current = EmotionController.Emotion.Neutral;
            GameEvents.RaiseEmotionChanged(EmotionController.Emotion.Neutral);
        }

        // 🔋 סטאמינה
        Stamina[] staminaComponents = GetComponentsInChildren<Stamina>(true);
        foreach (Stamina stamina in staminaComponents)
            stamina.ResetForNewScene();

        // 🔄 מפעילים מחדש סקריפטים
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
            script.enabled = true;

        // ✅ [חדש] מאפסים את ה-State Machine → חזרה ל-IdleState
        PlayerStateMachine sm = GetComponent<PlayerStateMachine>();
        if (sm != null)
            sm.ResetMachine();
    }
}
