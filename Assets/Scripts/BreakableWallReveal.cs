using UnityEngine;

// אבן מיוחדת שנשברת וחושפת אובייקט מוסתר
public class BreakableWallReveal : MonoBehaviour, IBreakable
{
    // האובייקט שיופיע אחרי השבירה
    [SerializeField] private GameObject hiddenObject;

    // פונקציה שמופעלת כשהאבן נשברת
    public void OnBreak()
    {
        // אם חיברנו אובייקט מוסתר - נפעיל אותו
        if (hiddenObject != null)
        {
            hiddenObject.SetActive(true);
            Debug.Log("Hidden object revealed.");
        }
        else
        {
            Debug.LogWarning("Hidden object is not assigned.");
        }

        Debug.Log("Reveal wall broken.");

        // משמיד את האבן
        Destroy(gameObject);
    }
}