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
    private float defaultY;

    private void Start()
    {
        defaultY = transform.position.y;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
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
        defaultY = transform.position.y;
        followY = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
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
            targetY = target.position.y + offset.y;
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