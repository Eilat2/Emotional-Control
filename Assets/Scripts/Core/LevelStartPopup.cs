using System.Collections;
using UnityEngine;

public class LevelStartPopup : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popup;

    [Header("Timing")]
    [SerializeField] private float showTime = 3f;

    private void Start()
    {
        StartCoroutine(ShowPopup());
    }

    private IEnumerator ShowPopup()
    {
        if (popup == null)
            yield break;

        popup.SetActive(true);

        yield return new WaitForSeconds(showTime);

        popup.SetActive(false);
    }
}
