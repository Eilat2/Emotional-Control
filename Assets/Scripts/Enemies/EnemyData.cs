using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Movement")]
    public float speed = 2f;
    public float reachDistance = 0.2f;

    [Header("Stamina Drain")]
    public float drainAmount = 15f; // כמה סטאמינה יורדת בפגיעה

    [Header("Hit Timing")]
    public float hitCooldown = 0.05f; // זמן בין פגיעות

    [Header("Hurt Lock + i-frames")]
    public float hurtLockTime = 0.18f;   // זמן שבו השחקן "נעול" אחרי פגיעה
    public float invincibleTime = 0.4f;  // זמן חסינות אחרי פגיעה

    [Header("Neutral Game Over Delay")]
    public float neutralGameOverDelay = 0.9f; // זמן לחכות כדי שיראו את הנוקבק וההבהוב לפני Game Over

    [Header("Rage Knockback")]
    public float rageKnockbackX = 10f;
    public float rageKnockbackY = 5f;

    [Header("Joy Reaction")]
    public EnemyTouchDamage.JoyHitReaction joyReaction = EnemyTouchDamage.JoyHitReaction.Knockback;

    [Header("Joy Knockback")]
    public float joyKnockbackX = 10f;
    public float joyKnockbackY = 6f;

    [Header("Joy Slam Down")]
    public float joySlamDownY = 12f;
    public float joyPushBackX = 0f;

    [Header("Ignore Stomp From Above")]
    public bool ignoreIfPlayerStompsFromAbove = false;
    public float stompTolerance = 0.5f;
}