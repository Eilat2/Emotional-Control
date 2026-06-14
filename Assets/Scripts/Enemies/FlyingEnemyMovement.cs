using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    private int currentPointIndex = 0;

    private void Awake()
    {
        if (enemyStats == null)
            Debug.LogError($"{gameObject.name}: EnemyStats is not assigned on FlyingEnemyMovement!");
    }

    private void Update()
    {
        if (enemyStats == null) return;
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            enemyStats.moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < enemyStats.reachDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}