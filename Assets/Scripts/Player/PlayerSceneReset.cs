using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneReset : MonoBehaviour
{
    // שומר את הגודל המקורי של השחקן
    private Vector3 originalScale;

    private void Awake()
    {
        // שומרים את הגודל ההתחלתי של השחקן
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
        // מחזירים את הגודל הרגיל של השחקן
        transform.localScale = originalScale;

        // מחזירים את כל הספרייטים לשקיפות רגילה
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        // מפעילים מחדש קוליידרים
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D col in colliders)
        {
            col.enabled = true;
        }

        // מאפסים Rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 1f;
            rb.simulated = true;
        }

        // מחזירים מצב רגשי לניטרלי
        EmotionController emotion = GetComponent<EmotionController>();
        if (emotion != null)
        {
            emotion.current = EmotionController.Emotion.Neutral;
        }

        // מאפסים סטאמינה
        Stamina[] staminaComponents = GetComponentsInChildren<Stamina>(true);
        Debug.Log("Found stamina components: " + staminaComponents.Length);

        foreach (Stamina stamina in staminaComponents)
        {
            stamina.ResetForNewScene();
            Debug.Log("Reset stamina: " + stamina.type + " -> " + stamina.currentStamina);
        }

        // מפעילים מחדש את כל הסקריפטים על השחקן
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }
}