using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 1;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float reachDistance = 0.2f;

    [Header("Damage To Player")]
    public float staminaDrain = 15f;

    [Header("Weaknesses")]
    public bool canBeStompedByJoy = false;
    public bool canBeDamagedByRage = false;

    [Header("Damage Received")]
    public int stompDamage = 1;
    public int rageDamage = 1;
}
