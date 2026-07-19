using UnityEngine;

public class ArmoredEnemyPatrol : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStats enemyStats;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    private Rigidbody2D _rb;
    private PatrolPointCycler _cycler;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (enemyStats == null)
            Debug.LogError($"{gameObject.name}: EnemyStats is not assigned on ArmoredEnemyPatrol!");

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: No patrol points assigned!");
            enabled = false;
            return;
        }

        _cycler = new PatrolPointCycler(patrolPoints);
    }

    private void FixedUpdate()
    {
        if (enemyStats == null)
            return;

        // מרחק על ציר X בלבד - כך אויב על הקרקע לא "נתקע" בגלל
        // הפרש גובה קטן בין נקודות הפטרול.
        float distanceX = Mathf.Abs(_cycler.Current.position.x - transform.position.x);
        Transform target = _cycler.AdvanceIfReached(distanceX, enemyStats.reachDistance);

        float direction = Mathf.Sign(target.position.x - transform.position.x);
        _rb.linearVelocity = new Vector2(direction * enemyStats.moveSpeed, _rb.linearVelocity.y);

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
