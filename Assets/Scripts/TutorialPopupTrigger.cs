using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
    [SerializeField] private GameObject popup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popup.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popup.SetActive(false);
        }
    }
}