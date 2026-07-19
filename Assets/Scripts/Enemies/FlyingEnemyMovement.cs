using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    private PatrolPointCycler _cycler;

    private void Awake()
    {
        if (enemyStats == null)
            Debug.LogError($"{gameObject.name}: EnemyStats is not assigned on FlyingEnemyMovement!");

        if (patrolPoints != null && patrolPoints.Length > 0)
            _cycler = new PatrolPointCycler(patrolPoints);
    }

    private void Update()
    {
        if (enemyStats == null || _cycler == null)
            return;

        Transform target = _cycler.Current;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            enemyStats.moveSpeed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, target.position);
        _cycler.AdvanceIfReached(distance, enemyStats.reachDistance);
    }
}
