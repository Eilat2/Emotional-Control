using UnityEngine;

public class KillableEnemy : MonoBehaviour, IBreakable
{
    [Header("Debris")]
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int debrisCount = 5;
    [SerializeField] private float force = 4f;
    [SerializeField] private float torque = 4f;

    private EnemyHealthSystem healthSystem;
    private bool isDead = false;

    private void Awake()
    {
        healthSystem = GetComponent<EnemyHealthSystem>();

        if (healthSystem == null)
            Debug.LogError($"{gameObject.name}: EnemyHealthSystem not found on KillableEnemy!");
    }

    public void OnBreak()
    {
        if (healthSystem != null)
            healthSystem.OnBreak();
        else
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        SpawnDebris();

        EnemyLevelCounter counter = FindFirstObjectByType<EnemyLevelCounter>();
        if (counter != null)
            counter.EnemyDied();

        Destroy(gameObject);
    }

    private void SpawnDebris()
    {
        if (debrisPrefab == null) return;

        for (int i = 0; i < debrisCount; i++)
        {
            GameObject piece = Instantiate(debrisPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 dir = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f)
                ).normalized;

                rb.AddForce(dir * force, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
            }
        }
    }
}