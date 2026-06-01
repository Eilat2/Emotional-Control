using UnityEngine;

public class Level3SecretElevator : MonoBehaviour
{
    [Header("נקודת יעד למעלה")]
    [SerializeField] private Transform topTargetPoint;

    [Header("נקודת יעד למטה")]
    [SerializeField] private Transform bottomTargetPoint;

    [Header("מהירות תנועה")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("מצלמה")]
    [SerializeField] private CameraFollow2D cameraFollow;

    [Header("הגובה המקסימלי של המצלמה בזמן עלייה")]
    [SerializeField] private float cameraMaxYWhenElevatorMoves = 120f;

    // האם אפשר להפעיל את המעלית לעלייה
    private bool canActivate = false;

    // האם אפשר להפעיל את המעלית לירידה
    private bool canReturnDown = false;

    // האם המעלית זזה כרגע
    private bool isMoving = false;

    // היעד הנוכחי שאליו המעלית זזה
    private Transform currentTarget;

    private void Start()
    {
        // אם לא חיברנו מצלמה ידנית, נחפש אותה בסצנה
        if (cameraFollow == null)
        {
            cameraFollow = FindFirstObjectByType<CameraFollow2D>();
        }
    }

    private void Update()
    {
        // אם המעלית לא זזה, אין מה לעדכן
        if (!isMoving) return;

        // אם אין יעד, אי אפשר להזיז את המעלית
        if (currentTarget == null) return;

        // מזיזים את המעלית לכיוון היעד הנוכחי
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget.position,
            moveSpeed * Time.deltaTime
        );

        // בודקים אם המעלית הגיעה ליעד
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            isMoving = false;

            Debug.Log("Elevator reached target");

            // אם הגענו חזרה לנקודה התחתונה,
            // מחזירים את המצלמה למעקב רגיל אחרי השחקן
            if (currentTarget == bottomTargetPoint && cameraFollow != null)
            {
                cameraFollow.ReturnToPlayer();
                cameraFollow.DisableFollowY();

                Debug.Log("Camera returned to player after elevator went down");
            }
        }
    }

    // נקרא אחרי שהפאזל הושלם
    public void UnlockElevator()
    {
        canActivate = true;

        Debug.Log("Elevator unlocked");
    }

    // נקרא אחרי שהמיני בוס מת
    public void UnlockReturnDown()
    {
        canReturnDown = true;

        Debug.Log("Elevator can now return down");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // רק השחקן יכול להפעיל את המעלית
        if (!collision.gameObject.CompareTag("Player")) return;

        // אם המעלית כבר זזה, לא מפעילים שוב
        if (isMoving) return;

        // אם אפשר לרדת, המעלית תרד
        if (canReturnDown)
        {
            StartMovingDown();
        }
        // אחרת, אם הפאזל הושלם, המעלית תעלה
        else if (canActivate)
        {
            StartMovingUp();
        }
    }

    // התחלת עלייה לחדר הסודי
    private void StartMovingUp()
    {
        currentTarget = topTargetPoint;
        isMoving = true;

        // בזמן העלייה, המצלמה עוברת לעקוב אחרי המעלית
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
            cameraFollow.SetMaxY(cameraMaxYWhenElevatorMoves);
            cameraFollow.EnableFollowY();
        }

        Debug.Log("Elevator moving up");
    }

    // התחלת ירידה חזרה למטה
    private void StartMovingDown()
    {
        currentTarget = bottomTargetPoint;
        isMoving = true;

        // בזמן הירידה, המצלמה עדיין עוקבת אחרי המעלית
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
            cameraFollow.EnableFollowY();
        }

        Debug.Log("Elevator moving down");
    }
}