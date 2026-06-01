using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject enemyToWatch;

    private void Update()
    {
        if (enemyToWatch == null)
        {
            if (popup != null)
                popup.SetActive(false);

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (enemyToWatch == null) return;

        if (popup != null)
            popup.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (popup != null)
            popup.SetActive(false);
    }
}