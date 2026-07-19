using System.Collections;
using UnityEngine;

public class CameraFocusSequence : MonoBehaviour
{
    [Header("הגדרות תנועה")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float waitAtFocusPoint = 1.5f;

    [Header("הגדרות זום")]
    [SerializeField] private float focusCameraSize = 20f;

    [Header("Snap Threshold")]
    [SerializeField] private float arrivalThreshold = 0.05f;

    // רפרנס לסקריפט המעקב הרגיל של המצלמה
    private CameraFollow2D _cameraFollow;

    // רפרנס לקומפוננטת המצלמה
    private Camera _cam;

    // מונע הפעלה כפולה של הסיקוונס
    private bool _isPlayingSequence;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _cameraFollow = GetComponent<CameraFollow2D>();
    }

    public void PlayFocusSequence(Transform focusTarget)
    {
        if (focusTarget == null || _isPlayingSequence)
            return;

        StartCoroutine(FocusSequenceCoroutine(focusTarget));
    }

    private IEnumerator FocusSequenceCoroutine(Transform focusTarget)
    {
        _isPlayingSequence = true;

        // מכבים זמנית את המעקב הרגיל של המצלמה
        if (_cameraFollow != null)
            _cameraFollow.enabled = false;

        // שומרים את המיקום והזום של המצלמה לפני הפוקוס
        Vector3 startPosition = transform.position;
        float startSize = _cam.orthographicSize;

        Vector3 targetPosition = new Vector3(
            focusTarget.position.x,
            focusTarget.position.y,
            transform.position.z
        );

        // תנועה אל אזור הפוקוס
        yield return MoveCameraTo(targetPosition, focusCameraSize);

        // מחכים רגע כדי להראות מה קרה
        yield return new WaitForSeconds(waitAtFocusPoint);

        // חוזרים לאיפה שהמצלמה הייתה קודם
        yield return MoveCameraTo(startPosition, startSize);

        // מדליקים מחדש את המעקב הרגיל
        if (_cameraFollow != null)
            _cameraFollow.enabled = true;

        _isPlayingSequence = false;
    }

    /// <summary>
    /// זז בחלקות למיקום ולזום המבוקשים. הקורוטינה הזו החליפה שני
    /// while-loops כמעט זהים (הלוך וחזור) שהיו בעבר משוכפלים בקובץ.
    /// </summary>
    private IEnumerator MoveCameraTo(Vector3 targetPosition, float targetSize)
    {
        while (Vector3.Distance(transform.position, targetPosition) > arrivalThreshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetSize, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // כדי לוודא שמגיעים בדיוק
        transform.position = targetPosition;
        _cam.orthographicSize = targetSize;
    }
}
