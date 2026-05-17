using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private DoorGlowController glow;
    [SerializeField] private Transform portalCenter;
    [SerializeField] private float suckDuration = 0.5f;
    [SerializeField] private bool fadePlayer = true;

    private bool canLoad = true;

    private readonly List<MonoBehaviour> disabledScripts = new List<MonoBehaviour>();

    private void Start()
    {
        // מחפש SceneFader אוטומטית אם לא חובר ידנית
        if (sceneFader == null)
        {
            sceneFader = FindFirstObjectByType<SceneFader>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canLoad) return;
        if (!other.CompareTag("Player")) return;

        canLoad = false;

        EmotionController emotion = other.GetComponent<EmotionController>();

        if (emotion != null && glow != null)
        {
            switch (emotion.GetCurrentEmotion())
            {
                case EmotionType.Joy:
                    glow.SetJoy();
                    break;

                case EmotionType.Rage:
                    glow.SetRage();
                    break;

                default:
                    glow.SetNeutral();
                    break;
            }
        }

        StartCoroutine(SuckPlayerAndLoad(other.gameObject));
    }

    private IEnumerator SuckPlayerAndLoad(GameObject player)
    {
        Transform playerTransform = player.transform;

        Vector3 startPos = playerTransform.position;
        Vector3 endPos = portalCenter != null ? portalCenter.position : transform.position;

        Vector3 startScale = playerTransform.localScale;
        Vector3 endScale = startScale * 0.2f;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        float originalGravity = 0f;

        if (rb != null)
            originalGravity = rb.gravityScale;

        SpriteRenderer[] renderers =
            player.GetComponentsInChildren<SpriteRenderer>(true);

        Color[] startColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            startColors[i] = renderers[i].color;
        }

        // מכבים שליטה זמנית
        DisablePlayerControl(player);

        float time = 0f;

        while (time < suckDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / suckDuration);

            // תנועה לשער
            playerTransform.position =
                Vector3.Lerp(startPos, endPos, t);

            // הקטנה
            playerTransform.localScale =
                Vector3.Lerp(startScale, endScale, t);

            // Fade לשחקן
            if (fadePlayer)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null) continue;

                    Color c = startColors[i];
                    c.a = Mathf.Lerp(startColors[i].a, 0f, t);

                    renderers[i].color = c;
                }
            }

            yield return null;
        }

        // מאפסים פיזיקה
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = originalGravity;
        }

        // מאפסים סטאמינה
        Stamina[] staminaComponents =
            player.GetComponentsInChildren<Stamina>(true);

        foreach (Stamina stamina in staminaComponents)
        {
            stamina.ResetForNewScene();
        }

        // מחזירים לניטרלי
        PlayerEmotionContext emotionContext =
            player.GetComponent<PlayerEmotionContext>();

        if (emotionContext != null)
        {
            emotionContext.ResetToNeutral();
        }

        // מחזירים את הסקריפטים שכיבינו
        foreach (MonoBehaviour script in disabledScripts)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // מעבר סצנה
        if (sceneFader != null)
        {
            sceneFader.FadeToScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void DisablePlayerControl(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
        }

        disabledScripts.Clear();

        MonoBehaviour[] scripts =
            player.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script is EmotionController) continue;
            if (script is PlayerEmotionContext) continue;
            if (script is PlayerSceneReset) continue;
            if (script is PersistentPlayer) continue;
            if (!script.enabled) continue;

            script.enabled = false;

            disabledScripts.Add(script);
        }
    }
}