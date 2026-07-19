using UnityEngine;

public class MovingElectricTilemapPlatform : MonoBehaviour
{
    [Header("נקודת יעד שמאל")]
    [SerializeField] private Transform leftTargetPoint;

    [Header("נקודת יעד ימין")]
    [SerializeField] private Transform rightTargetPoint;

    [Header("מהירות תנועה")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("האם להתחיל לכיוון ימין")]
    [SerializeField] private bool startMovingRight = true;

    private Transform _currentTarget;
    private float _startY;

    private void Start()
    {
        _startY = transform.position.y;
        _currentTarget = startMovingRight ? rightTargetPoint : leftTargetPoint;
    }

    private void Update()
    {
        if (leftTargetPoint == null || rightTargetPoint == null)
            return;

        Vector3 targetPosition = new Vector3(
            _currentTarget.position.x,
            _startY,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - _currentTarget.position.x) < 0.05f)
        {
            _currentTarget = _currentTarget == rightTargetPoint
                ? leftTargetPoint
                : rightTargetPoint;
        }
    }
}
