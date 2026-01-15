using System.Collections;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    public float flashDuration = 0.12f;

    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void Flash()
    {
        if (sr == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}
