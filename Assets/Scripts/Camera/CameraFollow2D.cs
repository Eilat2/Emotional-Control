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

    // האם כרגע המצלמה צריכה לעקוב גם אחרי Y של השחקן
    private bool followY = false;

    // מיקום ברירת המחדל של המצלמה
    private float defaultX;
    private float defaultY;

    private void Start()
    {
        // שומרים את מיקום ברירת המחדל של המצלמה בתחילת הסצנה
        defaultX = transform.position.x;
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
        // מאפסים את מיקום הבסיס של המצלמה בכל טעינת סצנה
        defaultX = transform.position.x;
        defaultY = transform.position.y;
        followY = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    // מפעיל מעקב אחרי הגובה של השחקן
    public void EnableFollowY()
    {
        followY = true;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // אם המצלמה במצב followY אבל השחקן ירד נמוך מספיק,
        // חוזרים למיקום הרגיל של המצלמה
        if (followY && target.position.y < returnBelowY)
        {
            followY = false;
        }

        float targetX = defaultX;
        float targetY;

        if (followY)
        {
            // בזמן מעקב לגובה - עוקבים אחרי Y של השחקן
            targetY = target.position.y + offset.y;

            // אם בעתיד תרצי גם לעקוב אחרי X, אפשר לשנות פה
            targetX = defaultX;
        }
        else
        {
            // במצב רגיל - חוזרים למיקום ברירת המחדל
            targetY = defaultY;
            targetX = defaultX;
        }

        // מגבילים את הגובה בין מינימום למקסימום
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