using UnityEngine;

public class CameraZoneUnlock : MonoBehaviour
{
    [SerializeField] private CameraFollow2D cameraFollow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (cameraFollow == null) return;

        cameraFollow.EnableFollowY();
    }
}