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
    private bool _canActivate;

    // האם אפשר להפעיל את המעלית לירידה
    private bool _canReturnDown;

    // האם המעלית זזה כרגע
    private bool _isMoving;

    // היעד הנוכחי שאליו המעלית זזה
    private Transform _currentTarget;

    private void Start()
    {
        // אם לא חיברנו מצלמה ידנית, נחפש אותה בסצנה
        if (cameraFollow == null)
            cameraFollow = FindFirstObjectByType<CameraFollow2D>();
    }

    private void Update()
    {
        if (!_isMoving || _currentTarget == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTarget.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _currentTarget.position) < 0.01f)
            HandleTargetReached();
    }

    private void HandleTargetReached()
    {
        _isMoving = false;
        StateLogger.Log(nameof(Level3SecretElevator), "Elevator reached target");

        // אם הגענו חזרה לנקודה התחתונה, מחזירים את המצלמה למעקב רגיל אחרי השחקן
        if (_currentTarget == bottomTargetPoint && cameraFollow != null)
        {
            cameraFollow.ReturnToPlayer();
            cameraFollow.DisableFollowY();
            StateLogger.Log(nameof(Level3SecretElevator), "Camera returned to player after elevator went down");
        }
    }

    // נקרא אחרי שהפאזל הושלם
    public void UnlockElevator()
    {
        _canActivate = true;
        StateLogger.Log(nameof(Level3SecretElevator), "Elevator unlocked");
    }

    // נקרא אחרי שהמיני בוס מת
    public void UnlockReturnDown()
    {
        _canReturnDown = true;
        StateLogger.Log(nameof(Level3SecretElevator), "Elevator can now return down");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // רק השחקן יכול להפעיל את המעלית
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // אם המעלית כבר זזה, לא מפעילים שוב
        if (_isMoving)
            return;

        if (_canReturnDown)
            StartMovingDown();
        else if (_canActivate)
            StartMovingUp();
    }

    // התחלת עלייה לחדר הסודי
    private void StartMovingUp()
    {
        _currentTarget = topTargetPoint;
        _isMoving = true;

        // בזמן העלייה, המצלמה עוברת לעקוב אחרי המעלית
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
            cameraFollow.SetMaxY(cameraMaxYWhenElevatorMoves);
            cameraFollow.EnableFollowY();
        }

        StateLogger.Log(nameof(Level3SecretElevator), "Elevator moving up");
    }

    // התחלת ירידה חזרה למטה
    private void StartMovingDown()
    {
        _currentTarget = bottomTargetPoint;
        _isMoving = true;

        // בזמן הירידה, המצלמה עדיין עוקבת אחרי המעלית
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
            cameraFollow.EnableFollowY();
        }

        StateLogger.Log(nameof(Level3SecretElevator), "Elevator moving down");
    }
}
