using UnityEngine;

// אבן מיוחדת שנשברת וחושפת אובייקט מוסתר
public class BreakableWallReveal : MonoBehaviour, IBreakable
{
    // האובייקט שיופיע אחרי השבירה
    [SerializeField] private GameObject hiddenObject;

    // פונקציה שמופעלת כשהאבן נשברת
    public void OnBreak()
    {
        if (hiddenObject != null)
        {
            hiddenObject.SetActive(true);
            StateLogger.Log(nameof(BreakableWallReveal), "Hidden object revealed.");
        }
        else
        {
            Debug.LogWarning("BreakableWallReveal: Hidden object is not assigned.");
        }

        Destroy(gameObject);
    }
}
