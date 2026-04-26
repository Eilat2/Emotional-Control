using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Collider2D blockingCollider;

    [Header("Behavior")]
    public bool disableOnOpen = false; // האם הדלת נעלמת כשפותחים

    public void OpenDoor()
    {
        Debug.Log("הדלת נפתחה!");

        if (blockingCollider != null)
            blockingCollider.enabled = false;

        // רק אם רוצים שהיא תיעלם
        if (disableOnOpen)
        {
            gameObject.SetActive(false);
        }
    }
}