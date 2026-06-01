using System.Collections;
using UnityEngine;

// אחראי רק על ההתנהגות של השחקן בזמן כניסה לדלת
public class PlayerDoorTransition : MonoBehaviour
{
    [SerializeField] private bool fadePlayer = true;

    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private MonoBehaviour[] scripts;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>(true);
        scripts = GetComponents<MonoBehaviour>();
    }

    public IEnumerator EnterDoor(Vector3 targetPosition, Vector3 targetScale, float duration)
    {
        FreezePlayer();

        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        Color[] startColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            startColors[i] = renderers[i].color;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // מזיזים את השחקן למרכז הדלת
            transform.position = Vector3.Lerp(startPos, targetPosition, t);

            // מקטינים אותו כדי שיראה כאילו הוא נבלע בדלת
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // מעלימים אותו בהדרגה
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

        transform.position = targetPosition;
        transform.localScale = targetScale;
    }

    private void FreezePlayer()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // מכבים קוליידרים כדי שלא ייתקע ברצפה או בקירות
        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // מכבים סקריפטים של השחקן כדי שלא ימשיכו להזיז אותו
        foreach (MonoBehaviour script in scripts)
        {
            if (script == null) continue;
            if (script == this) continue;

            script.enabled = false;
        }
    }
}