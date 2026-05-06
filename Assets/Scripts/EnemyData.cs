using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Stats")]
    public float moveSpeed = 2f;
    public int health = 1;

    [Header("Emotion Rules")]
    public bool canBeKilledByJoy;
    public bool canBeKilledByRage;
    public bool canBeKilledByNeutral;

    [Header("Feedback")]
    public ParticleSystem hitEffectPrefab;
    public AudioClip hitSound;
}