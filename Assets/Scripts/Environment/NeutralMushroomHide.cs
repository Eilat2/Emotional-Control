using UnityEngine;

public class NeutralMushroomHide : MonoBehaviour
{
    [SerializeField] GameObject hiddenButton;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        EmotionController ec = other.GetComponent<EmotionController>();
        if (ec == null) return;

        if (ec.current == EmotionController.Emotion.Neutral)
        {
            if (hiddenButton != null)
                hiddenButton.SetActive(true);

            gameObject.SetActive(false);
        }
    }
}