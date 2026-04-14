using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneReset : MonoBehaviour
{
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        EmotionController emotion = GetComponent<EmotionController>();
        if (emotion != null)
        {
            emotion.current = EmotionController.Emotion.Neutral;
        }

        Stamina[] staminaComponents = GetComponentsInChildren<Stamina>(true);
        Debug.Log("Found stamina components: " + staminaComponents.Length);

        foreach (Stamina stamina in staminaComponents)
        {
            stamina.ResetForNewScene();
            Debug.Log("Reset stamina: " + stamina.type + " -> " + stamina.currentStamina);
        }
    }
}