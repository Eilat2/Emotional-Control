using UnityEngine;

public class NeutralMushroomHide : MonoBehaviour
{
    [SerializeField] private GameObject hiddenButton;

    private void OnTriggerEnter2D(Collider2D other) => CheckReveal(other);
    private void OnTriggerStay2D(Collider2D other) => CheckReveal(other);

    private void CheckReveal(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        EmotionController emotion = other.GetComponent<EmotionController>();
        if (emotion == null || emotion.current != EmotionController.Emotion.Neutral)
            return;

        if (hiddenButton != null)
            hiddenButton.SetActive(true);

        gameObject.SetActive(false);
    }
}
