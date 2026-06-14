using UnityEngine;

public class ArmoredEnemyPatrol : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    private Rigidbody2D rb;
    private int currentPointIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemyStats == null)
            Debug.LogError($"{gameObject.name}: EnemyStats is not assigned on ArmoredEnemyPatrol!");

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: No patrol points assigned!");
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (enemyStats == null) return;

        Transform target = patrolPoints[currentPointIndex];
        float distanceX = Mathf.Abs(target.position.x - transform.position.x);

        if (distanceX <= enemyStats.reachDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            target = patrolPoints[currentPointIndex];
        }

        float direction = Mathf.Sign(target.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * enemyStats.moveSpeed, rb.linearVelocity.y);

        if (direction != 0)
            Flip(direction);
    }

    private void Flip(float direction)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }
}