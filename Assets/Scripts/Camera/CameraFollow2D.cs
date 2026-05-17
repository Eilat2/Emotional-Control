using UnityEngine;
using UnityEngine.SceneManagement;

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

    // האם כרגע המצלמה עוקבת אחרי מטרה מיוחדת כמו מעלית
    private bool followingSpecialTarget = false;

    private float defaultX;
    private float defaultY;

    private void Start()
    {
        defaultX = transform.position.x;
        defaultY = transform.position.y;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                target = player.transform;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        defaultX = transform.position.x;
        defaultY = transform.position.y;

        followY = false;
        followingSpecialTarget = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            target = player.transform;
    }

    public void EnableFollowY()
    {
        followY = true;
    }

    public void DisableFollowY()
    {
        followY = false;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        followingSpecialTarget = true;
    }

    public void ReturnToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            target = player.transform;

        followingSpecialTarget = false;
    }

    public void SetMaxY(float newMaxY)
    {
        maxY = newMaxY;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // מחזירים למצב רגיל רק כשהמטרה היא השחקן, לא כשזו מעלית
        if (!followingSpecialTarget && followY && target.position.y < returnBelowY)
        {
            followY = false;
        }

        float targetX = defaultX;
        float targetY;

        if (followY)
        {
            targetY = target.position.y + offset.y;
            targetX = defaultX;
        }
        else
        {
            targetY = defaultY;
            targetX = defaultX;
        }

        targetY = Mathf.Clamp(targetY, minY, maxY);

        Vector3 desiredPosition = new Vector3(
            targetX,
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