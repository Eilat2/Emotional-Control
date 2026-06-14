using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
    [SerializeField] private GameObject popup;

    private bool tutorialFinished = false;

    private void Start()
    {
        if (popup != null)
            popup.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tutorialFinished) return;
        if (!other.CompareTag("Player")) return;

        if (popup != null)
            popup.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (popup != null)
            popup.SetActive(false);
    }

    public void HidePopupForever()
    {
        tutorialFinished = true;

        if (popup != null)
            popup.SetActive(false);
    }
}