using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Collider2D blockingCollider;

    public void OpenDoor()
    {
        Debug.Log("הדלת נפתחה!");

        if (blockingCollider != null)
            blockingCollider.enabled = false;

        gameObject.SetActive(false);
    }
}