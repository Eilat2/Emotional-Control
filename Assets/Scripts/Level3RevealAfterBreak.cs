using UnityEngine;

public class Level3RevealAfterBreak : MonoBehaviour
{
    [Header("האובייקט שצריך להישבר")]
    [SerializeField] private GameObject objectToWatch;

    [Header("האובייקט שיופיע אחרי השבירה")]
    [SerializeField] private GameObject objectToReveal;

    private bool revealed = false;

    private void Start()
    {
        // החלק מוסתר בהתחלה
        if (objectToReveal != null)
            objectToReveal.SetActive(false);
    }

    private void Update()
    {
        if (revealed) return;

        // אם האובייקט נשבר / נעלם
        if (objectToWatch == null || !objectToWatch.activeInHierarchy)
        {
            revealed = true;

            Debug.Log("Level 3: Revealing puzzle piece");

            // חושפים את החלק
            if (objectToReveal != null)
                objectToReveal.SetActive(true);
        }
    }
}