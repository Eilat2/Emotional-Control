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

    private Transform currentTarget;
    private float startY;

    private void Start()
    {
        startY = transform.position.y;
        currentTarget = startMovingRight ? rightTargetPoint : leftTargetPoint;
    }

    private void Update()
    {
        if (leftTargetPoint == null || rightTargetPoint == null)
            return;

        Vector3 targetPosition = new Vector3(
            currentTarget.position.x,
            startY,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.05f)
        {
            currentTarget = currentTarget == rightTargetPoint
                ? leftTargetPoint
                : rightTargetPoint;
        }
    }
}