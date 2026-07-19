using UnityEngine;

public class KillableEnemy : MonoBehaviour, IBreakable
{
    [Header("Debris")]
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int debrisCount = 5;
    [SerializeField] private float force = 4f;
    [SerializeField] private float torque = 4f;

    private EnemyHealthSystem _healthSystem;
    private bool _isDead;

    private void Awake()
    {
        _healthSystem = GetComponent<EnemyHealthSystem>();

        if (_healthSystem == null)
            Debug.LogError($"{gameObject.name}: EnemyHealthSystem not found on KillableEnemy!");
    }

    public void OnBreak()
    {
        if (_healthSystem != null)
            _healthSystem.OnBreak();
        else
            Die();
    }

    public void Die()
    {
        if (_isDead)
            return;

        _isDead = true;

        SpawnDebris();

        EnemyLevelCounter counter = FindFirstObjectByType<EnemyLevelCounter>();
        counter?.EnemyDied();

        Destroy(gameObject);
    }

    private void SpawnDebris()
    {
        if (debrisPrefab == null)
            return;

        for (int i = 0; i < debrisCount; i++)
        {
            GameObject piece = Instantiate(debrisPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            if (rb == null)
                continue;

            Vector2 dir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f)
            ).normalized;

            rb.AddForce(dir * force, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
        }
    }
}
