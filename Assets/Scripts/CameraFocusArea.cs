using UnityEngine;

// אזור פוקוס למצלמה
// מגדיר למצלמה איפה להתמקד ומה גודל הזום הרצוי
public class CameraFocusArea : MonoBehaviour
{
    [Header("גודל המצלמה כשמגיעים לאזור הזה")]
    public float cameraSize = 20f;

    // מחזיר את המרכז של ה-BoxCollider2D
    public Vector3 GetCenter()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box != null)
        {
            return box.bounds.center;
        }

        return transform.position;
    }
}