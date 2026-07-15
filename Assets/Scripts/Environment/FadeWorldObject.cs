using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FadeWorldObject : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.4f;

    private SpriteRenderer[] spriteRenderers;
    private Tilemap[] tilemaps;
    private Collider2D[] colliders;
    private Coroutine fadeRoutine;
    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        tilemaps = GetComponentsInChildren<Tilemap>(true);
        colliders = GetComponentsInChildren<Collider2D>(true);

        initialized = true;
    }

    public void Show()
    {
        Initialize();

        gameObject.SetActive(true);
        SetAlpha(0f);
        SetColliders(true);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(Fade(0f, 1f));
    }

    public void Hide()
    {
        Initialize();

        if (!gameObject.activeInHierarchy)
        {
            SetAlpha(0f);
            SetColliders(false);
            gameObject.SetActive(false);
            return;
        }

        SetColliders(false);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return Fade(1f, 0f);
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, time / fadeDuration);
            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr == null) continue;

            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap == null) continue;

            Color c = tilemap.color;
            c.a = alpha;
            tilemap.color = c;
        }
    }

    private void SetColliders(bool enabled)
    {
        foreach (Collider2D col in colliders)
        {
            if (col == null) continue;
            col.enabled = enabled;
        }
    }
}