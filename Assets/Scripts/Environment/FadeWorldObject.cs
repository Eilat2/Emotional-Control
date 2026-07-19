using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FadeWorldObject : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.4f;

    private SpriteRenderer[] _spriteRenderers;
    private Tilemap[] _tilemaps;
    private Collider2D[] _colliders;
    private Coroutine _fadeRoutine;
    private bool _initialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        _tilemaps = GetComponentsInChildren<Tilemap>(true);
        _colliders = GetComponentsInChildren<Collider2D>(true);

        _initialized = true;
    }

    public void Show()
    {
        Initialize();

        gameObject.SetActive(true);
        SetAlpha(0f);
        SetColliders(true);

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(Fade(0f, 1f));
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

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(HideRoutine());
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
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            if (sr == null)
                continue;

            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        foreach (Tilemap tilemap in _tilemaps)
        {
            if (tilemap == null)
                continue;

            Color c = tilemap.color;
            c.a = alpha;
            tilemap.color = c;
        }
    }

    private void SetColliders(bool enabled)
    {
        foreach (Collider2D col in _colliders)
        {
            if (col == null)
                continue;

            col.enabled = enabled;
        }
    }
}
