using System.Collections;
using UnityEngine;

public class CameraFocusSequence : MonoBehaviour
{
    [Header("הגדרות תנועה")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float waitAtFocusPoint = 1.5f;

    [Header("הגדרות זום")]
    [SerializeField] private float focusCameraSize = 20f;

    // רפרנס לסקריפט המעקב הרגיל של המצלמה
    private CameraFollow2D cameraFollow;

    // רפרנס לקומפוננטת המצלמה
    private Camera cam;

    // מונע הפעלה כפולה של הסיקוונס
    private bool isPlayingSequence = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cameraFollow = GetComponent<CameraFollow2D>();
    }

    public void PlayFocusSequence(Transform focusTarget)
    {
        if (focusTarget == null || isPlayingSequence)
            return;

        StartCoroutine(FocusSequenceCoroutine(focusTarget));
    }

    private IEnumerator FocusSequenceCoroutine(Transform focusTarget)
    {
        isPlayingSequence = true;

        // מכבים זמנית את המעקב הרגיל של המצלמה
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        // שומרים את המיקום והזום של המצלמה לפני הפוקוס
        Vector3 startPosition = transform.position;
        float startSize = cam.orthographicSize;

        // לאיזה מקום המצלמה צריכה להגיע
        Vector3 targetPosition = new Vector3(
            focusTarget.position.x,
            focusTarget.position.y,
            transform.position.z
        );

        // תנועה אל אזור הפוקוס
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, focusCameraSize, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // כדי לוודא שמגיעים בדיוק
        transform.position = targetPosition;
        cam.orthographicSize = focusCameraSize;

        // מחכים רגע כדי להראות מה קרה
        yield return new WaitForSeconds(waitAtFocusPoint);

        // חוזרים לאיפה שהמצלמה הייתה קודם
        while (Vector3.Distance(transform.position, startPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, moveSpeed * Time.deltaTime);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, startSize, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // כדי לוודא שחזרנו בדיוק
        transform.position = startPosition;
        cam.orthographicSize = startSize;

        // מדליקים מחדש את המעקב הרגיל
        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }

        isPlayingSequence = false;
    }
}