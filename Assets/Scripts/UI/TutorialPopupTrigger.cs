using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
    [SerializeField] private GameObject popup;

    private bool _tutorialFinished;

    private void Start()
    {
        if (popup != null)
            popup.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_tutorialFinished || !other.CompareTag("Player"))
            return;

        if (popup != null)
            popup.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (popup != null)
            popup.SetActive(false);
    }

    public void HidePopupForever()
    {
        _tutorialFinished = true;

        if (popup != null)
            popup.SetActive(false);
    }
}
