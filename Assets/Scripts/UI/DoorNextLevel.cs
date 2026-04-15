using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName;          // שם הסצנה הבאה
    [SerializeField] private SceneFader sceneFader;         // מעבר עם Fade
    [SerializeField] private DoorGlowController glow;       // glow של הדלת
    [SerializeField] private Transform portalCenter;        // נקודה במרכז השער
    [SerializeField] private float suckDuration = 0.5f;     // כמה זמן היניקה תיקח
    [SerializeField] private bool fadePlayer = true;        // האם להעלים את השחקן תוך כדי

    private bool canLoad = true;

    private readonly List<MonoBehaviour> disabledScripts = new List<MonoBehaviour>();

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
        Vector3 endScale = startScale * 0.2f; // קטן תוך כדי

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        float originalGravity = 0f;

        if (rb != null)
            originalGravity = rb.gravityScale;

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        Color startColor = Color.white;

        if (sr != null)
            startColor = sr.color;

        // מכבים שליטה רק אחרי ששמרנו את המצב המקורי
        DisablePlayerControl(player);

        float time = 0f;

        while (time < suckDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / suckDuration);

            // תנועה חלקה למרכז השער
            playerTransform.position = Vector3.Lerp(startPos, endPos, t);

            // הקטנה הדרגתית
            playerTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            // העלמה הדרגתית
            if (fadePlayer && sr != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                sr.color = c;
            }

            yield return null;
        }

        // מחזירים רק פיזיקה רגילה, כדי שהשחקן לא יישאר "מקולקל"
        // אבל לא מחזירים גודל/שקיפות כאן, כדי שלא יראו אותו קופץ החוצה
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = originalGravity;
        }

        // מאפסים את כל רכיבי הסטאמינה שעל השחקנית
        Stamina[] staminaComponents = player.GetComponents<Stamina>();
        foreach (Stamina stamina in staminaComponents)
        {
            stamina.ResetForNewScene();
        }

        // מעבר סצנה
        if (sceneFader != null)
        {
            sceneFader.FadeToScene(nextSceneName);
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

        // מכבים את כל ה-MonoBehaviourים של השחקן חוץ מהדברים שלא רוצים לכבות
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script is EmotionController) continue;
            if (!script.enabled) continue;

            script.enabled = false;
            disabledScripts.Add(script);
        }
    }
}