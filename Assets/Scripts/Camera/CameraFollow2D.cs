using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Vertical Limits")]
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 8f;

    [Header("Return Settings")]
    [SerializeField] private float returnBelowY = 2f;

    private bool followY = false;
    private float defaultY;

    private void Start()
    {
        defaultY = transform.position.y;
    }

    public void EnableFollowY()
    {
        followY = true;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (followY && target.position.y < returnBelowY)
        {
            followY = false;
        }

        float targetY;

        if (followY)
        {
            targetY = Mathf.Clamp(target.position.y + offset.y, minY, maxY);
        }
        else
        {
            targetY = defaultY;
        }

        Vector3 desiredPosition = new Vector3(
            transform.position.x,
            targetY,
            offset.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}