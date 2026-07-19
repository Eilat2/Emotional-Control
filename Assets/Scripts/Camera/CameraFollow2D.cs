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

    private bool _followY;

    // האם כרגע המצלמה עוקבת אחרי מטרה מיוחדת כמו מעלית
    private bool _followingSpecialTarget;

    private float _defaultX;
    private float _defaultY;

    private void Start()
    {
        _defaultX = transform.position.x;
        _defaultY = transform.position.y;

        if (target == null)
            FindPlayer();
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
        _defaultX = transform.position.x;
        _defaultY = transform.position.y;

        _followY = false;
        _followingSpecialTarget = false;

        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            target = player.transform;
    }

    public void EnableFollowY() => _followY = true;
    public void DisableFollowY() => _followY = false;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _followingSpecialTarget = true;
    }

    public void ReturnToPlayer()
    {
        FindPlayer();
        _followingSpecialTarget = false;
    }

    public void SetMaxY(float newMaxY) => maxY = newMaxY;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // מחזירים למצב רגיל רק כשהמטרה היא השחקן, לא כשזו מעלית
        if (!_followingSpecialTarget && _followY && target.position.y < returnBelowY)
            _followY = false;

        // ה-X נשאר תמיד קבוע (defaultX) - גם כשעוקבים אנכית וגם לא.
        float targetX = _defaultX;
        float targetY = _followY ? target.position.y + offset.y : _defaultY;

        targetY = Mathf.Clamp(targetY, minY, maxY);

        Vector3 desiredPosition = new Vector3(targetX, targetY, offset.z);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
