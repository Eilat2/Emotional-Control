using System.Collections;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer neutralRenderer;
    [SerializeField] private SpriteRenderer joyRenderer;
    [SerializeField] private SpriteRenderer rageRenderer;

    [Header("Visuals")]
    [SerializeField] private float blinkStepDuration = 0.06f;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private float squashScale = 0.85f;

    [Header("References")]
    [SerializeField] private EmotionController emotionController;
    [SerializeField] private Transform neutralVisual;
    [SerializeField] private Transform joyVisual;
    [SerializeField] private Transform rageVisual;

    public void PlayHitFeedback()
    {
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        SpriteRenderer activeRenderer = GetActiveRenderer();
        Transform activeVisual = GetActiveVisual();

        if (activeRenderer == null || activeVisual == null || emotionController == null)
            yield break;

        Vector3 originalScale = activeVisual.localScale;

        for (int i = 0; i < blinkCount; i++)
        {
            activeVisual.localScale = originalScale * squashScale;
            activeRenderer.enabled = false;
            yield return new WaitForSeconds(blinkStepDuration);

            activeRenderer.enabled = true;
            activeVisual.localScale = originalScale;
            yield return new WaitForSeconds(blinkStepDuration);
        }

        activeRenderer.enabled = true;
        activeVisual.localScale = originalScale;
    }

    private SpriteRenderer GetActiveRenderer()
    {
        switch (emotionController.current)
        {
            case EmotionController.Emotion.Joy:
                return joyRenderer;
            case EmotionController.Emotion.Rage:
                return rageRenderer;
            default:
                return neutralRenderer;
        }
    }

    private Transform GetActiveVisual()
    {
        switch (emotionController.current)
        {
            case EmotionController.Emotion.Joy:
                return joyVisual;
            case EmotionController.Emotion.Rage:
                return rageVisual;
            default:
                return neutralVisual;
        }
    }
}