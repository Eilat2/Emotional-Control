using UnityEngine;

public class DynamicCameraDeathZone : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Position")]
    [SerializeField] private float offsetBelowCamera = 1.5f;

    [Header("Size")]
    [SerializeField] private float zoneHeight = 1f;
    [SerializeField] private float zoneWidthExtra = 5f;

    private BoxCollider2D _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        _boxCollider.isTrigger = true;
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        float bottomY = targetCamera.transform.position.y - cameraHeight / 2f;

        transform.position = new Vector3(
            targetCamera.transform.position.x,
            bottomY - offsetBelowCamera,
            0f
        );

        _boxCollider.size = new Vector2(cameraWidth + zoneWidthExtra, zoneHeight);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // משתמשים ב-GameEvents הקיים (במקום FindObjectOfType יקר)
        StateLogger.Log(nameof(DynamicCameraDeathZone), "Player fell below camera death zone.");
        GameEvents.RaiseGameOver();
    }
}
