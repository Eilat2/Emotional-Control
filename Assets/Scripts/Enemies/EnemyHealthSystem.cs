using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour, IBreakable
{
    [SerializeField] private EnemyStats enemyStats;

    [Header("HP Visuals")]
    [SerializeField] private GameObject[] heartVisuals;

    private int currentHealth;

    private void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogError($"{gameObject.name}: EnemyStats not assigned on EnemyHealthSystem!");
            return;
        }

        currentHealth = enemyStats.maxHealth;
        UpdateHeartVisuals();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} took {damageAmount} damage. HP: {currentHealth}/{enemyStats.maxHealth}");

        UpdateHeartVisuals();

        if (currentHealth <= 0)
            Die();
    }

    // נקרא מ-KillableEnemy.OnBreak() דרך IBreakable — מטפל בנזק Rage
    public void OnBreak()
    {
        if (enemyStats == null || !enemyStats.canBeDamagedByRage) return;
        TakeDamage(enemyStats.rageDamage);
    }

    private void UpdateHeartVisuals()
    {
        for (int i = 0; i < heartVisuals.Length; i++)
        {
            if (heartVisuals[i] == null) continue;
            heartVisuals[i].SetActive(i < currentHealth);
        }
    }

    private void Die()
    {
        // מעביר ל-KillableEnemy שאחראי על Debris + LevelCounter + Destroy
        KillableEnemy killable = GetComponent<KillableEnemy>();
        if (killable != null)
            killable.Die();
        else
            Destroy(gameObject);
    }
}